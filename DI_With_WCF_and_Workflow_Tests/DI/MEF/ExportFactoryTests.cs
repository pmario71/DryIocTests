using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DI_With_WCF_and_Workflow_Tests.DI.MEF
{
    [TestFixture]
    public class ExportFactoryTests
    {

        [Test]
        public void Factory_can_be_injected_using_constructor_injection()
        {
            var cat = new TypeCatalog(new[] {typeof(Foo)});
            var container = new CompositionContainer(cat, CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);

            var creator = container.GetExportedValue<Creator>();

            Assert.NotNull(creator.Factory);
        }

        [Test]
        public void Factory_can_be_injected_using_property_injection()
        {
            var cat = new TypeCatalog(new[] { typeof(Foo) });
            var container = new CompositionContainer(cat, CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);

            var creator = container.GetExportedValue<Creator2>();

            Assert.NotNull(creator.Factory);
        }

        [Test]
        public void Factory_can_be_injected_using_property_injection_using_Compose()
        {
            var cat = new TypeCatalog(new[] { typeof(Foo) });
            var container = new CompositionContainer(cat, CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);

            var creator = new Creator2();

            container.ComposeParts(creator);

            Assert.NotNull(creator.Factory);
        }


        [Export]
        class Creator
        {
            public ExportFactory<IFoo> Factory { get; }

            [ImportingConstructor]
            public Creator(ExportFactory<IFoo> factory)
            {
                Factory = factory;
            }
        }

        class Creator2
        {
            [Import]
            public ExportFactory<IFoo> Factory { get; set; }
        }

    }

    internal interface IFoo
    {
    }

    [Export(typeof(IFoo))]
    class Foo : IFoo
    {
        
    }
}
