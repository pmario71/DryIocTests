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
    public class Host<TContract, TService> : IDisposable
        where TService : TContract
    {
        readonly List<IDisposable> _resources = new List<IDisposable>();

        public void Initialize()
        {
            var cat = new AssemblyCatalog(typeof(Host<TContract, TService>).Assembly);

            var provider = new CatalogContainerProvider(cat);
            OpenService(provider);
        }

        public Uri Address { get; private set; }

        private void OpenService(IContainerProvider provider)
        {
            var svcHost = new ComposedServiceHost<TService>(provider);

            Uri uri = AddressFromContract();
            svcHost.AddServiceEndpoint(typeof(TContract), DefaultBinding, uri);

            svcHost.Open();
            this.Address = uri;
        }

        public static Binding DefaultBinding
        {
            get { return new NetNamedPipeBinding(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Uri AddressFromContract(string id=null)
        {
            string rand = id ?? Guid.NewGuid().ToString();

            return new Uri($"net.pipe://localhost/{typeof(TContract).Name}_{rand}");
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
            if (errors)
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
