using System;
using System.ComponentModel.Composition.Primitives;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace DI_With_WCF_and_Workflow.DI.MEF
{
    /// <summary>
    /// ServiceHost that supports resolving dependencies via MEF.
    /// </summary>
    public sealed class ComposedServiceHost<T> : ServiceHost
    {
        //private readonly Type               _serviceType;
        private readonly IContainerProvider _provider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType">The type of hosted service.</param>
        /// <param name="catalogProvider">MEF catalog used to resolve dependencies from.</param>
        /// <param name="baseAddresses">An array of type <see cref="T:System.Uri"/> that contains the base addresses for the hosted service.</param>
        public ComposedServiceHost(Func<ComposablePartCatalog> catalogProvider, params Uri[] baseAddresses)
            : this(new CatalogContainerProvider(catalogProvider()), baseAddresses)
        {
        }

        public ComposedServiceHost(IContainerProvider provider, params Uri[] baseAddresses)
            : base(typeof(T), baseAddresses)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _provider = provider;
        }

        /// <summary>
        /// Not for public use!
        /// </summary>
        protected override void OnOpening()
        {
            if (Description.Behaviors.Find<ComposedServiceBehavior<T>>() == null)
            {
                Description.Behaviors.Add(new ComposedServiceBehavior<T>(_provider));
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