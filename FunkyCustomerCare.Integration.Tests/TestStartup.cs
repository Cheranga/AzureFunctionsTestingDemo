using System;
using System.Collections.Generic;
using System.Text;
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

            var services = builder.Services;

            services.AddSingleton(provider =>
            {
                var configuration = GetConfigurationRoot(builder);
                var localRuntimeSettings = new LocalRuntimeSettings();
                configuration.GetSection("Values:LocalRuntimeSettings").Bind(localRuntimeSettings);

                return localRuntimeSettings;
            });
        }
    }

    public class LocalRuntimeSettings
    {
        public string DotnetExecutablePath { get; set; }
        public string FunctionHostPath { get; set; }
        public string FunctionApplicationPath { get; set; }
    }
}
