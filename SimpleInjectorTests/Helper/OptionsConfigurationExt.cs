using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInjectorTests
{
    public static class OptionsConfigurationExt
    {

        public static void AddOptions(this Container container)
        {
            container.Register(
                typeof(Microsoft.Extensions.Options.IOptions<>),
                typeof(Microsoft.Extensions.Options.OptionsManager<>), Lifestyle.Singleton);

            container.Register(
                typeof(Microsoft.Extensions.Options.IOptionsMonitor<>),
                typeof(Microsoft.Extensions.Options.OptionsMonitor<>), Lifestyle.Singleton);

            container.Register(
                typeof(Microsoft.Extensions.Options.IOptionsSnapshot<>),
                typeof(Microsoft.Extensions.Options.OptionsSnapshot<>), Lifestyle.Scoped);
        }

        /// <summary>
        /// Registers an action used to configure a particular type of options.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="services">The <see cref="Container"/> to add the services to.</param>
        /// <param name="configureOptions">The action used to configure the options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static Container Configure<TOptions>(this Container services, Action<TOptions> configureOptions)
            where TOptions : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            IEnumerable<IConfigureOptions<TOptions>> configColl = null;

            //services.RegisterConditional<IEnumerable<IConfigureOptions<TOptions>>>(                );

            new List<IConfigureOptions<TOptions>>();
            services.Register<IConfigureOptions<TOptions>>(
                () => new ConfigureOptions<TOptions>(configureOptions),
                Lifestyle.Singleton);

            services.Register<IEnumerable<IConfigureOptions<TOptions>>>();

            return services;
        }

        public static Container Configure<TOptions>(this Container services, IConfigurationSection section)
            where TOptions : class
        {
            Action<TOptions> options = _ => ConfigurationBinder.Get<TOptions>(section);
            return Configure(services, options);
        }

        public static Container ConfigureCustom<TOptions>(this Container services, IConfigurationSection section)
            where TOptions : class
        {
            Action<TOptions> options = _ => ConfigurationBinder.Get<TOptions>(section);


            return Configure(services, options);
        }
    }
}
