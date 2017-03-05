using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using DI_With_WCF_and_Workflow.DI.MEF;
using DI_With_WCF_and_Workflow;
using DI_With_WCF_and_Workflow.WCFServices;
using JetBrains.dotMemoryUnit;

namespace DI_With_WCF_and_Workflow_Tests.DI.MEF
{
    [TestFixture]
    public class ComposedServiceHostTests
    {

        [Test]
        public void Can_resolve_dependencies()
        {
            Func<ComposablePartCatalog> provider = () => new AssemblyCatalog(typeof(StringConverter).Assembly);
            var sut = new ComposedServiceHost<TestService>(provider);

            var binding = new NetNamedPipeBinding();
            var address = Host<ITestWCFService, TestWCFService>.AddressFromContract();
            sut.AddServiceEndpoint(typeof(IService), binding, address);

            sut.Open();

            var proxy = ChannelFactory<IService>.CreateChannel(binding, new EndpointAddress(address));

            Assert.AreEqual("cba", (string)proxy.ResolveDependency("abc"));
        }

        [Test]
        public void Sharing_of_services_is_possible()
        {
            var container = new CompositionContainer(new AssemblyCatalog(typeof(StringConverter).Assembly),
                CompositionOptions.DisableSilentRejection);

            var provider = new ContainerProvider(container);

            var sut = new ComposedServiceHost<TestService>(provider);

            var binding = new NetNamedPipeBinding();
            var address = Host<ITestWCFService, TestWCFService>.AddressFromContract();
            sut.AddServiceEndpoint(typeof(IService), binding, address);

            sut.Open();

            var proxy = ChannelFactory<IService>.CreateChannel(binding, new EndpointAddress(address));

            Assert.AreEqual("cba", (string)proxy.ResolveDependency("abc"));
        }

        [Test, Explicit]
        [DotMemoryUnit(FailIfRunWithoutSupport = false)]
        public void Ressources_are_freed_again()
        {
            Func<ComposablePartCatalog> provider = () => new AssemblyCatalog(typeof(StringConverter).Assembly);
            var sut = new ComposedServiceHost<TestService>(provider);

            var binding = new NetNamedPipeBinding();
            var address = Host<ITestWCFService, TestWCFService>.AddressFromContract();
            sut.AddServiceEndpoint(typeof(IService), binding, address);

            sut.Open();

            //for (int i = 0; i < 2000; i++)
            //{
            //    CreateClientAndCallService(binding, address);
            //}

            Parallel.For(0, 2000, _ =>
            {
                CreateClientAndCallService(binding, address);
            });

            Thread.Sleep(1000);

            dotMemory.Check(memory =>
            {
                Assert.That(memory.GetObjects(where => where.Type.Is<TestService>()).ObjectsCount, Is.EqualTo(1));
            } );
        }

        private static void CreateClientAndCallService(NetNamedPipeBinding binding, Uri address)
        {
            var proxy = ChannelFactory<IService>.CreateChannel(binding, new EndpointAddress(address));

            proxy.ResolveDependency("abc");

            var d = (IDisposable)proxy;
            d.Dispose();
        }

        class ContainerProvider : IContainerProvider
        {
            private readonly CompositionContainer _container;

            public ContainerProvider(CompositionContainer container)
            {
                _container = container;
            }

            public CompositionContainer GetContainer()
            {
                return _container;
            }
        }
    }

    [ServiceContract(Namespace = Defines.NS, Name = Defines.Name)]
    public interface IService
    {
        [OperationContract]
        string ResolveDependency(string input);
    }

    public static class Defines
    {
        public const string NS = "http://www.tests.com/";
        public const string Name = "IService";
    }

    [Export]
    public class StringConverter
    {
        public string Convert(string input)
        {
            return string.Join("", input.Reverse());
        }
    }

    [Export]
    public class TestService : IService
    {
        private readonly StringConverter converter;

        [ImportingConstructor]
        public TestService(StringConverter converter)
        {
            this.converter = converter;
        }

        public string ResolveDependency(string input)
        {
            return converter.Convert(input);
        }
    }

}
