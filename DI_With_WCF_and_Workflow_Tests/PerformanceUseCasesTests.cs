/* -------------------------------------------------------------------------------------------------
   Restricted - Copyright (C) Siemens Healthcare GmbH/Siemens Medical Solutions USA, Inc., 2017. All rights reserved
   ------------------------------------------------------------------------------------------------- */
   
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using DI_With_WCF_and_Workflow.DI.MEF;
using DI_With_WCF_and_Workflow.WCFServices;

using DI_With_WCF_and_Workflow_Tests.Utils;

using HdrHistogram;

using NUnit.Framework;


namespace DI_With_WCF_and_Workflow_Tests
{
    [TestFixture(Explicit = true)]
    public class PerformanceUseCasesTests
    {
        private List<IDisposable> _cleanupList = new List<IDisposable>();

        [TearDown]
        public void Dispose()
        {
            if (_cleanupList.Any())
            {
                _cleanupList.Reverse();
                foreach (var c in _cleanupList)
                {
                    c.Dispose();
                }
                _cleanupList.Clear();
            }
        }


        [Test]
        public async Task Call_composed_service_once()
        {
            var sw = Stopwatch.StartNew();

            var p = DemoUseCases.CreateContainerProvider();
            Uri uri = Helper.UriFromContract<ITestWCFService>();

            var h = StartHost<ITestWCFService, TestWCFService>(p, uri);

            var client = CreateClient<ITestWCFService>(uri);

            await client.ExecuteShortRunningOperationAsync(0);

            Console.WriteLine($"Calling composed service once: {sw.ElapsedMilliseconds} [ms]");
        }

        [Test]
        public void Call_composed_service_multiple_times()
        {
            var histogramFactory = HdrHistogram.HistogramFactory.With32BitBucketSize();
            var hist = histogramFactory.Create();

            var p = DemoUseCases.CreateContainerProvider();
            Uri uri = Helper.UriFromContract<ITestWCFService>();

            var h = StartHost<ITestWCFService, TestWCFService>(p, uri);

            var client = CreateClient<ITestWCFService>(uri);

            for (int i = 0; i < 100; i++)
            {
                var sw = Stopwatch.StartNew();

                hist.Record(() =>
                {
                    int sss =client.ExecuteShortRunningOperationAsync(0).Result;
                });
            }
            var txtWriter = new StringWriter();
            hist.OutputPercentileDistribution(txtWriter);

            Console.WriteLine($"Values are in ticks => devide by to get [ms]: {Stopwatch.Frequency}");
            Console.WriteLine(txtWriter.GetStringBuilder().ToString());

        }


        private TContract CreateClient<TContract>(Uri uri)
        {
            var client = ChannelFactory<TContract>.CreateChannel(
                new NetNamedPipeBinding(),
                new EndpointAddress(uri));

            _cleanupList.Add((IDisposable)client);

            return client;
        }

        

        private ComposedServiceHost StartHost<TContract, TService>(IContainerProvider provider, Uri uri)
           where TService : TContract
        {
            var svcHost = new ComposedServiceHost(typeof(TService), provider);

            svcHost.AddServiceEndpoint(typeof(TContract), new NetNamedPipeBinding(), uri);

            svcHost.Open();

            _cleanupList.Add(svcHost);

            return svcHost;
        }
    }
}
