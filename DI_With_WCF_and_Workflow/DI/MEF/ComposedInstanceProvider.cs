using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace DI_With_WCF_and_Workflow.DI.MEF
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", 
        Justification = "WCF does not use IDisposable but calls ReleaseInstance() to trigger freeing of resources.")]
    internal class ComposedInstanceProvider : IInstanceProvider
    {
        private readonly Type                 _serviceType;
        private readonly CompositionContainer _container;

        public ComposedInstanceProvider(Type serviceType, IContainerProvider containerProvider)
        {
            if (containerProvider == null)
                throw new ArgumentNullException(nameof(containerProvider));

            _serviceType = serviceType;
            _container = containerProvider.GetContainer();
        }

        public object GetInstance(InstanceContext context)
        {
            Export export = GetServiceExport();

            if (export == null)
            {
                string msg = $"Failed to instantiate type '{_serviceType.Name}'! Most likely it is not declared as export!";
                throw new InvalidOperationException(msg);
            }

            return export.Value;
        }

        public object GetInstance(InstanceContext context, Message message)
        {
            return GetInstance(context);
        }

        public void ReleaseInstance(InstanceContext context, object instance)
        {
            var disposable = instance as IDisposable;

            if (disposable != null)
                disposable.Dispose();

            _container.Dispose();
        }

        private Export GetServiceExport()
        {
            var importDefinition
                = new ContractBasedImportDefinition(AttributedModelServices.GetContractName(_serviceType),
                    AttributedModelServices.GetTypeIdentity(_serviceType),
                    null,
                    ImportCardinality.ZeroOrMore,
                    true,
                    true,
                    CreationPolicy.NonShared);

            return _container.GetExports(importDefinition).FirstOrDefault();
        }
    }
}
