using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using FunkyCustomerCare.Config;
using FunkyCustomerCare.Core;
using FunkyCustomerCare.Models;
using Microsoft.Extensions.Logging;

namespace FunkyCustomerCare.Services
{
    public class CreateBlobService : ICreateBlobService
    {
        private readonly ILogger<CreateBlobService> _logger;
        private readonly StorageAccountConfiguration _storageAccountConfiguration;

        public CreateBlobService(StorageAccountConfiguration storageAccountConfiguration, ILogger<CreateBlobService> logger)
        {
            _storageAccountConfiguration = storageAccountConfiguration;
            _logger = logger;
        }

        public async Task<Result> CreateBlobAsync(CreateBlobRequest request)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(_storageAccountConfiguration.ConnectionString);

                var blobContainerClient = blobServiceClient.GetBlobContainerClient(request.Container);
                
                // Create the BLOB container if it does not exist
                await blobContainerClient.CreateIfNotExistsAsync();
                

                var blobClient = blobContainerClient.GetBlobClient(request.FileName);

                var memoryStream = new MemoryStream();
                var streamWriter = new StreamWriter(memoryStream) {AutoFlush = true};

                await streamWriter.WriteAsync(request.Content);
                memoryStream.Position = 0;

                var uploadResponse = await blobClient.UploadAsync(memoryStream, true, CancellationToken.None);

                var isCreated = uploadResponse?.GetRawResponse()?.Status == (int) HttpStatusCode.Created;

                return isCreated ? Result.Success() : Result.Failure("Error when creating the blob.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error when creating the BLOB");
            }

            return Result.Failure("Error when creating the blob.");
        }
    }
}