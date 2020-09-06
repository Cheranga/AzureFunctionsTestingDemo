using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using FunkyCustomerCare;
using FunkyCustomerCare.Config;
using FunkyCustomerCare.Functions;
using FunkyCustomerCare.Services;
using FunkyCustomerCare.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
                var config = new CustomerApiConfig();
                configuration.GetSection("Values:CustomerApiConfig").Bind(config);

                return config;
            });

            services.AddSingleton(provider =>
            {
                var config = new StorageAccountConfiguration();
                configuration.GetSection("Values:StorageAccountConfiguration").Bind(config);

                return config;
            });

            services.AddHttpClient<ICategorizeCustomerService, CategorizeCustomerService>((provider, client) =>
            {
                var config = provider.GetRequiredService<CustomerApiConfig>();
                client.BaseAddress = new Uri(config?.Url);
            });

            services.AddSingleton<ICreateBlobService, CreateBlobService>();

            services.AddValidatorsFromAssembly(typeof(Startup).Assembly, ServiceLifetime.Singleton);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            RegisterDependencies(builder);
        }
    }
}
