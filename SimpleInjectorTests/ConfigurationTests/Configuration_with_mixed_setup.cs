using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInjectorTests.ConfigurationTests
{
    [TestFixture]
    public class Configuration_with_mixed_setup
    {



        

    }

    class BootStrapper
    {
        private object Configuration;

        public void Initialize()
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
        }
    }
}
