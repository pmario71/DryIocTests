using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace DI_With_WCF_and_Workflow.DI.MEF
{
    public class CatalogContainerProvider : IContainerProvider
    {
        private readonly ComposablePartCatalog[] _catalogs;
        private readonly TypeCatalog _rootCat = new TypeCatalog(new[] { typeof(DisposeTracker) });
        
        /// <summary>
        /// Creates a ContainerProvider that uses a hierarchy of catalogs to initialize each
        /// container instance it creates.
        /// Types exported from catalogs later in the sequence, do override previously registered types.
        /// </summary>
        /// <param name="overrideCatalogs"></param>
        public CatalogContainerProvider(
            params ComposablePartCatalog[] catalogHierarchy)
        {
            _catalogs    = catalogHierarchy;
        }

        public CompositionContainer GetContainer()
        {
            CatalogExportProvider[] wrappedCatalogs = WrapCatalogsIntoExportProviders(_catalogs);

            var container = new CompositionContainer(
                _rootCat,
                CompositionOptions.DisableSilentRejection,
                wrappedCatalogs);

            var tracker = container.GetExportedValue<DisposeTracker>();

            foreach (var wrappedCat in wrappedCatalogs)
            {
                wrappedCat.SourceProvider = container;
                tracker.RegisterForDisposal(wrappedCat);
            }

            ContainerInstanceCreated?.Invoke(container);

            return container;
        }

        public Action<CompositionContainer> ContainerInstanceCreated { set; get; }

        private CatalogExportProvider[] WrapCatalogsIntoExportProviders(ComposablePartCatalog[] catalog)
        {
            return catalog
                .Reverse()  // CompositionContainer's rule for Service precedence is inverse
                .Select(c => new CatalogExportProvider(c))
                .ToArray();
        }

        [Export]
        class DisposeTracker : IDisposable
        {
            private List<IDisposable> _providers = new List<IDisposable>();

            public void RegisterForDisposal(params IDisposable[] disposables)
            {
                _providers.AddRange(disposables);
            }

            public void Dispose()
            {
                if (_providers != null)
                {
                    foreach (var p in _providers)
                    {
                        p.Dispose();
                    }
                    _providers = null;
                }
            }
        }
    }


    // Implementing Scopes with MEF
    // http://stackoverflow.com/questions/16943121/defining-scope-in-mef-with-compositionscopedefinition

}