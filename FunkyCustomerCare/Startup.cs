using System;
using System.Collections.Generic;
using System.Text;
using FunkyCustomerCare;
using FunkyCustomerCare.Config;
using FunkyCustomerCare.Functions;
using FunkyCustomerCare.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly:FunctionsStartup(typeof(Startup))]
namespace FunkyCustomerCare
{
    public class Startup : FunctionsStartup
    {
        protected virtual IConfigurationRoot GetConfigurationRoot(IFunctionsHostBuilder functionsHostBuilder)
        {
            var services = functionsHostBuilder.Services;

            var executionContextOptions = services
                .BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>()
                .Value;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(executionContextOptions.AppDirectory)
                .AddJsonFile("local.settings.json", true)
                .AddEnvironmentVariables()
                .Build();

            return configuration;
        }

        protected virtual void RegisterDependencies(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            var configuration = GetConfigurationRoot(builder);

            services.AddSingleton(provider =>
            {
                var customerApiConfig = new CustomerApiConfig();
                configuration.GetSection("Values:CustomerApiConfig").Bind(customerApiConfig);

                return customerApiConfig;
            });

            services.AddHttpClient<ICategorizeCustomerService, CategorizeCustomerService>((provider, client) =>
            {
                var config = provider.GetRequiredService<CustomerApiConfig>();
                client.BaseAddress = new Uri(config?.Url);
            });

            services.AddSingleton<ICreateBlobService, CreateBlobService>();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            RegisterDependencies(builder);
        }
    }
}
