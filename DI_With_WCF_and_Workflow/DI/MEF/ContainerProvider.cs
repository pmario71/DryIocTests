namespace DI_With_WCF_and_Workflow.DI.MEF
{

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