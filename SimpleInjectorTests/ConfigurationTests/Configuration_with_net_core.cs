using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInjectorTests.ConfigurationTests
{
    [TestFixture]
    public class Configuration_with_net_core
    {
        [Test]
        public void Can_create_controller_with_logger()
        {
            var dict = new Dictionary<string, string>
            {
                {"Profile:MachineName", "Rick"},
                {"TestConfig:Url", "http://localhost:9988"},
            };

            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(dict);
            var config = builder.Build();

            var c = new Container();
            c.AddOptions();
            c.Configure<TestConfig>(config.GetSection("TestConfig"));

            var xxx = config.GetSection("TestConfig").Get<TestConfig>();


            var sut = c.GetInstance<ConfiguredService>();

            Assert.IsNotNull(sut);
            Assert.AreEqual(dict["TestConfig:Url"], sut.Config.Url);
        }

        class ConfiguredService
        {
            private TestConfig _config;

            public ConfiguredService(IOptions<TestConfig> config)
            {
                _config = config.Value;
            }

            public TestConfig Config => _config;
        }
    }

    public class TestConfig
    {
        public TestConfig()
        {
            int i = 0;
        }

        public string Url { get; set; }
    }
}
