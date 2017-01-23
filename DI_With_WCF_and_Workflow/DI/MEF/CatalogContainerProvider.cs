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
            return new CompositionContainer(_catalog, CompositionOptions.DisableSilentRejection);
        }
    }
}