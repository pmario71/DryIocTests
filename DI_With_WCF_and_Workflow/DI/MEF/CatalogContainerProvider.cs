using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace DI_With_WCF_and_Workflow.DI.MEF
{
    public class CatalogContainerProvider : IContainerProvider
    {
        private readonly ComposablePartCatalog _catalog;
        

        public CatalogContainerProvider(ComposablePartCatalog catalog)
        {
            _catalog = catalog;
        }

        public CompositionContainer GetContainer()
        {
            var container = new CompositionContainer(_catalog, CompositionOptions.DisableSilentRejection);

            ContainerInstanceCreated?.Invoke(container);

            return container;
        }

        public Action<CompositionContainer> ContainerInstanceCreated { set; get; }
    }


    // Implementing Scopes with MEF
    // http://stackoverflow.com/questions/16943121/defining-scope-in-mef-with-compositionscopedefinition

}