using System;
using System.IO;
using System.Threading.Tasks;
using FunkyCustomerCare.Core;
using FunkyCustomerCare.Functions;
using FunkyCustomerCare.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunkyCustomerCare.Services
{
    public class CreateBlobService : ICreateBlobService
    {
        private readonly ILogger<CreateBlobService> _logger;

        public CreateBlobService(ILogger<CreateBlobService> logger)
        {
            _logger = logger;
        }

        public async Task<Result> CreateBlobAsync(IBinder binder, CreateBlobRequest request)
        {
            try
            {
                var filePath = $"{request.Container}/{request.FileName}";
                var dynamicBlobBinding = new BlobAttribute(filePath, FileAccess.Write);

                using (var writer = binder.Bind<TextWriter>(dynamicBlobBinding))
                {
                    await writer.WriteAsync(request.Content);
                }

                return Result.Success();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error when creating the BLOB");
            }

            return Result.Failure("Error when creating the blob.");
        }
    }
}