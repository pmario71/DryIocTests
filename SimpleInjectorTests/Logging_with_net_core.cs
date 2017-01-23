using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInjectorTests
{
    public class Logging_with_net_core
    {
        [Test]
        public void Can_create_controller_with_logger()
        {
            var c = new Container();
            c.Register<ILoggerFactory,LoggerFactory>(Lifestyle.Singleton);
            c.Register(typeof(ILogger<>), typeof(Logger<>));
            c.Register<DummyController>();

            var factory = c.GetInstance<ILoggerFactory>(); //new Microsoft.Extensions.Logging.LoggerFactory();
            factory.AddConsole();

            var controller = c.GetInstance<DummyController>();
        }
    }

    class DummyController
    {
        private ILogger<DummyController> _logger;

        public DummyController(ILogger<DummyController> logger)
        {
            _logger = logger;
            _logger.LogInformation("DummyController created!");
        }
    }

}
