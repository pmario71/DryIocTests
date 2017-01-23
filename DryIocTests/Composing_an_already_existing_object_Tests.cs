using DryIoc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DryIocTests
{
    [TestFixture]
    public class Composing_an_already_existing_object_Tests
    {
        [Test]
        public void PropertyInjection()
        {
            var c = new Container();
            c.Register<IService, SomeService>();

            var obj = new PropertyInjectionTestClass();
            c.InjectPropertiesAndFields(obj);

            Assert.IsNotNull(obj.Service);
        }

    }

    class PropertyInjectionTestClass
    {
        public IService Service { get; set; }
    }
}
