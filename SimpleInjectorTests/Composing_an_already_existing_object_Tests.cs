using NUnit.Framework;
using SimpleInjector;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInjectorTests
{
    [TestFixture]
    public class Composing_an_already_existing_object_Tests
    {
        [Test]
        public void PropertyInjection()
        {
            var c = new Container();
            c.Options.PropertySelectionBehavior = new ImplicitPropertyInjectionBehavior(c);
            c.Register<IService, SomeService>();

            var obj = new PropertyInjectionTestClass();

            Registration registration = Lifestyle.Transient.CreateRegistration(typeof(PropertyInjectionTestClass), c);
            registration.InitializeInstance(obj);

            Assert.IsNotNull(obj.Service);
        }

    }

    class PropertyInjectionTestClass
    {
        public IService Service { get; set; }
    }
}
