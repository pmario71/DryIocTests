using DryIoc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DryIocTests
{
    public class Logging_with_net_core
    {
        [Test]
        public void Can_create_controller_with_logger()
        {
            var c = new Container();
            c.Register<ILoggerFactory,LoggerFactory>(Reuse.Singleton);
            c.Register(typeof(ILogger<>), typeof(Logger<>));

            var factory = c.Resolve<ILoggerFactory>(); //new Microsoft.Extensions.Logging.LoggerFactory();
            factory.AddConsole();

            c.Register<DummyController>();

            var controller = c.Resolve<DummyController>();
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
