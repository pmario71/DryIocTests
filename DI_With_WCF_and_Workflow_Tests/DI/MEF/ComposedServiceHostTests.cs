using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DI_With_WCF_and_Workflow.DI.MEF;

namespace DI_With_WCF_and_Workflow_Tests.DI.MEF
{
    [TestFixture]
    public class ComposedServiceHostTests
    {

        [Test,Explicit("Do not run tests in parallel, because they open the same endpoint!")]
        public void Can_resolve_dependencies()
        {
            Func<ComposablePartCatalog> provider = () => new AssemblyCatalog(typeof(StringConverter).Assembly);
            var sut = new ComposedServiceHost(typeof(TestService), provider);

            var binding = new NetNamedPipeBinding();
            var address = new Uri("net.pipe://localhost/" + "IService");
            sut.AddServiceEndpoint(typeof(IService), binding, address);

            sut.Open();

            var proxy = ChannelFactory<IService>.CreateChannel(binding, new EndpointAddress(address));

            Assert.AreEqual("cba", (string)proxy.ResolveDependency("abc"));
        }

        [Test, Explicit("Do not run tests in parallel, because they open the same endpoint!")]
        public void Sharing_of_services_is_possible()
        {
            var container = new CompositionContainer(new AssemblyCatalog(typeof (StringConverter).Assembly),
                CompositionOptions.DisableSilentRejection);

            var provider = new ContainerProvider(container);

            var sut = new ComposedServiceHost(typeof(TestService), provider);

            var binding = new NetNamedPipeBinding();
            var address = new Uri("net.pipe://localhost/" + "IService");
            sut.AddServiceEndpoint(typeof(IService), binding, address);

            sut.Open();

            var proxy = ChannelFactory<IService>.CreateChannel(binding, new EndpointAddress(address));

            Assert.AreEqual("cba", (string)proxy.ResolveDependency("abc"));
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
