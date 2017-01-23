using DI_With_WCF_and_Workflow.DI.MEF;
using DI_With_WCF_and_Workflow.WCFServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace DI_With_WCF_and_Workflow
{
    public class Host : IDisposable
    {
        List<IDisposable> _resources = new List<IDisposable>();

        public static void Main()
        {

        }

        public void Initialize()
        {
            var cat = new AssemblyCatalog(typeof(Host).Assembly);

            var provider = new CatalogContainerProvider(cat);
            OpenService<ITestWCFService,TestWCFService>(provider);
        }

        private static void OpenService<TContract, TService>(IContainerProvider provider)
            where TService : TContract
        {
            var svcHost = new ComposedServiceHost(typeof(TService), provider);

            svcHost.AddServiceEndpoint(typeof(TContract), DefaultBinding, AddressFromContract<TContract>());

            svcHost.Open();
        }

        public static Binding DefaultBinding
        {
            get { return new NetNamedPipeBinding(); }
        }
        public static Uri AddressFromContract<TContract>()
        {
            return new Uri(string.Format("net.pipe://localhost/{0}", typeof(TContract).Name));
        }

        public void Dispose()
        {
            var cleanupList = _resources.ToArray();
            _resources.Clear();

            bool errors = false;
            foreach (var resource in cleanupList)
            {
                errors |= SafeDispose(resource);
            }
            Console.WriteLine("Errors while shutting down!");
        }

        private bool SafeDispose(IDisposable resource)
        {
            try
            {
                resource.Dispose();
            }
            catch (Exception)
            {
                return true;
            }
            return false;
        }
    }
}
