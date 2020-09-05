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
    public class RegisterCustomerFunction
    {
        private readonly IRegisterCustomerService _registerCustomerService;
        private readonly ICreateBlobService _blobService;
        private readonly ILogger<RegisterCustomerFunction> _logger;

        public RegisterCustomerFunction(IRegisterCustomerService registerCustomerService, ICreateBlobService blobService, ILogger<RegisterCustomerFunction> logger)
        {
            _registerCustomerService = registerCustomerService;
            _blobService = blobService;
            _logger = logger;
        }

        [FunctionName(nameof(RegisterCustomerFunction))]
        public async Task<IActionResult> CreateAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "customers")]
            HttpRequest request,
            IBinder binder)
        {
            var operation = await request.GetModel<RegisterCustomerRequest>();
            if (!operation.Status)
            {
                return new BadRequestResult();
            }

            var registerCustomerOperation = await _registerCustomerService.RegisterAsync(operation.Data);
            if (!registerCustomerOperation.Status)
            {
                return new InternalServerErrorResult();
            }

            if (registerCustomerOperation.Data == CustomerCategory.Vip)
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