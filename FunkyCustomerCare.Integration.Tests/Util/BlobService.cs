using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FunkyCustomerCare.Integration.Tests.Util
{
    public class BlobService
    {
        private readonly CloudBlobClient _blobClient;

        public BlobService()
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<string> GetContentAsync(string container, string fileName)
        {
            try
            {
                var blobContainer = _blobClient.GetContainerReference(container);

                var containerExists = await blobContainer.ExistsAsync();

                if (!containerExists)
                {
                    return string.Empty;
                }

                var blob = blobContainer.GetBlobReference(fileName);
                var blobExists = await blob.ExistsAsync();

                if (!blobExists)
                {
                    return string.Empty;
                }

                using (var stream = new MemoryStream())
                {
                    await blob.DownloadToStreamAsync(stream);
                    stream.Position = 0;

                    using (var reader = new StreamReader(stream))
                    {
                        var content = await reader.ReadToEndAsync();

                        return content;
                    }
                }
            }
            catch
            {
                // Ignored
            }

            return string.Empty;
        }

        public async Task<bool> DeleteBlobsAsync()
        {
            try
            {
                var blobServiceClient = new BlobServiceClient("UseDevelopmentStorage=true");
                var vipContainer = blobServiceClient.GetBlobContainerClient("vip-customers");
                var regularContainer = blobServiceClient.GetBlobContainerClient("regular-customers");

                var blobs = vipContainer.GetBlobsAsync();
                await foreach (var item in blobs)
                {
                    await vipContainer.DeleteBlobAsync(item.Name);
                }

                blobs = regularContainer.GetBlobsAsync();
                await foreach (var item in blobs)
                {
                    await regularContainer.DeleteBlobAsync(item.Name);
                }
            }
            catch
            {
                // Ignored
            }

            return false;
        }
    }
}
