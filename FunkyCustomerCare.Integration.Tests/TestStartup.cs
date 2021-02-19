using System;
using System.Collections.Generic;
using System.Text;
using FunkyCustomerCare.Functions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FunkyCustomerCare.Integration.Tests
{
    public class TestStartup : Startup
    {
        protected override IConfigurationRoot GetConfigurationRoot(IFunctionsHostBuilder functionsHostBuilder)
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("local.settings.json")
                .AddEnvironmentVariables()
                .Build();

            return configuration;
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            base.Configure(builder);

            //
            // If required override the dependencies here
            //
            var services = builder.Services;

            services.AddTransient<CategorizeCustomerFunction>();
        }
    }
}
