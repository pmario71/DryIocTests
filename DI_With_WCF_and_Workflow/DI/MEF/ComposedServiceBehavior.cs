using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace DI_With_WCF_and_Workflow.DI.MEF
{
    internal class ComposedServiceBehavior<T> : IServiceBehavior
    {
        private readonly IInstanceProvider _instanceProvider;

        public ComposedServiceBehavior(IContainerProvider container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            _instanceProvider = new ComposedInstanceProvider<T>();

            container.GetContainer().ComposeParts(_instanceProvider);
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase dispatcher in serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = dispatcher as ChannelDispatcher;

                if (channelDispatcher == null)
                    continue;

                foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                {
                    endpointDispatcher.DispatchRuntime.InstanceProvider = _instanceProvider;
                }
            }
        }
    }
}