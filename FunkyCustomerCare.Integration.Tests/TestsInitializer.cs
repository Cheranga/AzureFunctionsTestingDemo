using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using AutoFixture;
using FunkyCustomerCare.Integration.Tests.Util;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RimDev.Automation.StorageEmulator;

namespace FunkyCustomerCare.Integration.Tests
{
    public class TestsInitializer : IDisposable
    {
        private AzureStorageEmulatorAutomation _emulator;

        public TestsInitializer()
        {
            var host = new HostBuilder()
                .ConfigureWebJobs(builder => builder.UseWebJobsStartup<TestStartup>())
                .Build();

            ServiceProvider = host.Services;

            Fixture = new Fixture();
            

            TestDataReader = new AssemblyResourceFileReader();

            BlobService = new BlobService();

            StartLocalRuntime();
        }

        private void StartLocalRuntime()
        {
            _emulator = new AzureStorageEmulatorAutomation();

            if (!AzureStorageEmulatorAutomation.IsEmulatorRunning())
            {
                _emulator.Start();
            }

        }

        public BlobService BlobService { get; set; }

        public AssemblyResourceFileReader TestDataReader { get; set; }

        public IServiceProvider ServiceProvider { get; }
        public Fixture Fixture { get; }

        public void Dispose()
        {
            if (_emulator != null)
            {
                BlobService.DeleteBlobsAsync().Wait();
                //_emulator.Stop();
            }
        }
    }
}