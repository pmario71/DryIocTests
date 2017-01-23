using System;
using System.ComponentModel.Composition.Primitives;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace DI_With_WCF_and_Workflow.DI.MEF
{
    /// <summary>
    /// ServiceHost that supports resolving dependencies via MEF.
    /// </summary>
    public sealed class ComposedServiceHost : ServiceHost
    {
        private readonly Type               _serviceType;
        private readonly IContainerProvider _provider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType">The type of hosted service.</param>
        /// <param name="catalogProvider">MEF catalog used to resolve dependencies from.</param>
        /// <param name="baseAddresses">An array of type <see cref="T:System.Uri"/> that contains the base addresses for the hosted service.</param>
        public ComposedServiceHost(Type serviceType, Func<ComposablePartCatalog> catalogProvider, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            if (catalogProvider == null)
                throw new ArgumentNullException(nameof(catalogProvider));

            _serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            _provider = new CatalogContainerProvider(catalogProvider());
        }

        public ComposedServiceHost(Type serviceType, IContainerProvider provider, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            _serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        /// Not for public use!
        /// </summary>
        protected override void OnOpening()
        {
            if (Description.Behaviors.Find<ComposedServiceBehavior>() == null)
            {
                Description.Behaviors.Add(new ComposedServiceBehavior(_serviceType, _provider));
            }

            var debugBehavior = Description.Behaviors.Find<ServiceDebugBehavior>();
            if (debugBehavior == null)
            {
                debugBehavior=new ServiceDebugBehavior();
                Description.Behaviors.Add(debugBehavior);
            }
            debugBehavior.IncludeExceptionDetailInFaults = true;

            base.OnOpening();
        }
    }

}