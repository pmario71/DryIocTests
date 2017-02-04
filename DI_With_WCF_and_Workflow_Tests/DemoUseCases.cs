using DI_With_WCF_and_Workflow;
using DI_With_WCF_and_Workflow.DI.MEF;
using DI_With_WCF_and_Workflow.WCFServices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DI_With_WCF_and_Workflow_Tests
{
    [TestFixture(Explicit =true)]
    public class DemoUseCases
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
        public async Task Call_simple_operation_using_async_API()
        {
            var p = CreateContainerProvider();
            Uri uri = Helper.UriFromContract<ITestWCFService>();

            var h = StartHost<ITestWCFService,TestWCFService>(p, uri);

            var client = CreateClient(uri);
            await client.ExecuteShortRunningOperationAsync(300);
        }

        [Test]
        public async Task Call_operation_which_uses_internally_a_transaction()
        {
            var p = CreateContainerProvider();
            Uri uri = Helper.UriFromContract<ITestWCFService>();

            var h = StartHost<ITestWCFService, TestWCFService>(p, uri);

            var client = CreateClient(uri);
            await client.ExecuteOperationUsingTransactionAsync(300);
        }

        [Test]
        public async Task Resolved_dependencies_are_disposed()
        {
            var p = CreateContainerProvider();

            // hook container creation
            CompositionContainer container = null;
            ((CatalogContainerProvider)p).ContainerInstanceCreated = ci => container = ci;

            Uri uri = Helper.UriFromContract<ITestWCFService2>();

            var h = StartHost<ITestWCFService2, TestWCFService2>(p, uri);


            var dependency = container.GetExportedValue<IHashcodeProvider>();

            var client = CreateClient<ITestWCFService2>(uri);

            var res = await client.ResolveSharedResourceAsync();

            ((IDisposable)client).Dispose();
            h.Close();

            Assert.AreEqual(dependency.GetHashCode(), res.DependencyInstanceId, "Dependency was not shared!");
            Assert.IsTrue(dependency.IsDisposed, "Shared dependency was not disposed!");
        }

        [Test]
        public async Task Call_operation_which_depends_on_a_shared_resource()
        {
            var p = CreateContainerProvider();
            Uri uri = Helper.UriFromContract<ITestWCFService>();

            var h = StartHost<ITestWCFService, TestWCFService>(p, uri);

            var client1 = CreateClient(uri);
            var client2 = CreateClient(uri);

            var instanceID1 = await client1.ResolveSharedResourceAsync();
            var instanceID2 = await client2.ResolveSharedResourceAsync();

            Assert.AreNotEqual(instanceID1.ServiceInstanceId, instanceID2.ServiceInstanceId);
            Assert.AreEqual(instanceID1.DependencyInstanceId, instanceID2.DependencyInstanceId);
        }

        [Test]
        public async Task Ressources_are_not_shared_between_different_services()
        {
            // test shows in addition that service registrations can be reused across different
            // service types, while instance lifecycle management is separated

            var p = CreateContainerProvider();
            Uri uri = Helper.UriFromContract<ITestWCFService>();

            StartHost<ITestWCFService, TestWCFService>(p, uri);

            Uri uri2 = Helper.UriFromContract<ITestWCFService2>();
            StartHost<ITestWCFService2, TestWCFService2>(p, uri2);

            var client1 = CreateClient(uri);
            var client2 = CreateClient<ITestWCFService2>(uri2);

            var instanceID1 = await client1.ResolveSharedResourceAsync();
            var instanceID2 = await client2.ResolveSharedResourceAsync();

            Assert.AreNotEqual(instanceID1.ServiceInstanceId, instanceID2.ServiceInstanceId);
            Assert.AreNotEqual(instanceID1.DependencyInstanceId, instanceID2.DependencyInstanceId);
        }

        [Test]
        public async Task ServiceRegistrations_can_be_shared_and_overridden_between_different_services()
        {
            var cat = new AssemblyCatalog(typeof(TestWCFService).Assembly);
            var typeCat = new TypeCatalog(new[] { typeof(OverriddenHashcodeProvider) });

            var provider = new CatalogContainerProvider(cat, typeCat);


            Uri uri = Helper.UriFromContract<ITestWCFService>();

            StartHost<ITestWCFService, TestWCFService>(provider, uri);

            var client1 = CreateClient(uri);

            var instanceID1 = await client1.ResolveSharedResourceAsync();


            // overridden HashcodeProvider returns -1
            Assert.AreEqual(-1, instanceID1.DependencyInstanceId);
        }

        #region Internals
        private ITestWCFService CreateClient(Uri uri)
        {
            return CreateClient<ITestWCFService>(uri);
        }

        private TContract CreateClient<TContract>(Uri uri)
        {
            var client = ChannelFactory<TContract>.CreateChannel(
                new NetNamedPipeBinding(),
                new EndpointAddress(uri));

            _cleanupList.Add((IDisposable)client);

            return client;
        }

        private IContainerProvider CreateContainerProvider()
        {
            var cat = new AssemblyCatalog(typeof(TestWCFService).Assembly);

            var provider = new CatalogContainerProvider(cat);
            return provider;
        }

        private ComposedServiceHost StartHost<TContract,TService>(IContainerProvider provider, Uri uri)
            where TService : TContract
        {
            var svcHost = new ComposedServiceHost(typeof(TService), provider);

            svcHost.AddServiceEndpoint(typeof(TContract), new NetNamedPipeBinding(), uri);

            svcHost.Open();

            _cleanupList.Add(svcHost);

            return svcHost;
        }

        [Export(typeof(IHashcodeProvider))]
        [PartCreationPolicy(CreationPolicy.Shared)]
        class OverriddenHashcodeProvider : IHashcodeProvider
        {
            public int GetInstanceId()
            {
                return -1;
            }

            public bool IsDisposed { get; private set; }
        }

        #endregion
    }
}
