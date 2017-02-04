using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Registration;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_With_WCF_and_Workflow_Tests.DI.MEF
{
    [TestFixture]
    public class MEFTests
    {

        [Test]
        public void Service_registrations_can_be_overridden()
        {
            // implementing natural overrides, ExportProviders need to be introduced otherwise a
            // CardinalityMismatchException is thrown

            var tc1 = new TypeCatalog(new[] { typeof(Svc1), typeof(Svc3) });
            var tc2 = new TypeCatalog(new[] {  typeof(Svc2) });

            var ep1 = new CatalogExportProvider(tc1);
            var ep2 = new CatalogExportProvider(tc2);

            var cont = new CompositionContainer(CompositionOptions.DisableSilentRejection, ep2, ep1);

            ep1.SourceProvider = cont;
            ep2.SourceProvider = cont;

            var res = cont.GetExportedValue<ISvc>();

            Assert.IsInstanceOf<Svc2>(res);

            // ensure that exports from ep1 are still resolvable
            var res2 = cont.GetExportedValue<Svc3>();
        }

        [Test]
        public void Services_are_disposed()
        {
            // implementing natural overrides, ExportProviders need to be introduced otherwise a
            // CardinalityMismatchException is thrown

            var tcRoot = new TypeCatalog(new[] { typeof(DisposeTracker) });
            var tc1 = new TypeCatalog(new[] { typeof(Svc1), typeof(Svc3) });
            var tc2 = new TypeCatalog(new[] { typeof(Svc2) });

            var ep1 = new CatalogExportProvider(tc1);
            var ep2 = new CatalogExportProvider(tc2);

            var cont = new CompositionContainer(tcRoot, CompositionOptions.DisableSilentRejection, ep2, ep1);

            ep1.SourceProvider = cont;
            ep2.SourceProvider = cont;

            var tracker = cont.GetExportedValue<DisposeTracker>();
            tracker.RegisterForDisposal(ep1,ep2);


            var res = cont.GetExportedValue<ISvc>();

            cont.Dispose();

            Assert.True(((Svc2)res).IsDisposed);
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

        [Export(typeof(ISvc))]
        public class Svc1 : ISvc, IDisposable
        {
            public bool IsDisposed { get; private set; }
            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        [Export(typeof(ISvc))]
        public class Svc2 : ISvc, IDisposable
        {
            public bool IsDisposed { get; private set; }
            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        public interface ISvc
        {
        }

        [Export]
        public class Svc3
        {
        }
    }

    
}
