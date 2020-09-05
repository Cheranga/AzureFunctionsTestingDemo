using System.Threading.Tasks;
using System.Web.Http;
using FunkyCustomerCare.DTO;
using FunkyCustomerCare.Extensions;
using FunkyCustomerCare.Models;
using FunkyCustomerCare.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace FunkyCustomerCare.Functions
{
    public class CategorizeCustomerFunction
    {
        private readonly ICategorizeCustomerService _categorizeCustomerService;
        private readonly ICreateBlobService _blobService;
        private readonly ILogger<CategorizeCustomerFunction> _logger;

        public CategorizeCustomerFunction(ICategorizeCustomerService categorizeCustomerService, ICreateBlobService blobService, ILogger<CategorizeCustomerFunction> logger)
        {
            _categorizeCustomerService = categorizeCustomerService;
            _blobService = blobService;
            _logger = logger;
        }

        [FunctionName(nameof(CategorizeCustomerFunction))]
        public async Task<IActionResult> CreateAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "customers")]
            HttpRequest request,
            IBinder binder)
        {
            var operation = await request.GetModel<CategorizeCustomerRequest>();
            if (!operation.Status)
            {
                return new BadRequestResult();
            }

            var categorizeCustomerOperation = await _categorizeCustomerService.CategorizeAsync(operation.Data);
            if (!categorizeCustomerOperation.Status)
            {
                return new InternalServerErrorResult();
            }

            if (categorizeCustomerOperation.Data == CustomerCategory.Vip)
            {
                await _blobService.CreateBlobAsync(binder, new CreateBlobRequest("vip-customers", operation.Data.Id, JsonConvert.SerializeObject(operation.Data)));
            }

            else
            {
                await _blobService.CreateBlobAsync(binder, new CreateBlobRequest("regular-customers", operation.Data.Id, JsonConvert.SerializeObject(operation.Data)));
            }

            return new AcceptedResult();
        }
    }
}