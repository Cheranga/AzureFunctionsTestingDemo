using System;
using System.Net.Http;
using System.Threading.Tasks;
using FunkyCustomerCare.Core;
using FunkyCustomerCare.DTO;
using Microsoft.Extensions.Logging;

namespace FunkyCustomerCare.Services
{
    public class RegisterCustomerService : IRegisterCustomerService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RegisterCustomerService> _logger;

        public RegisterCustomerService(HttpClient httpClient, ILogger<RegisterCustomerService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Result<CustomerCategory>> RegisterAsync(RegisterCustomerRequest request)
        {
            try
            {
                //
                // TODO: Implement the service where it create
                //

                await Task.Delay(TimeSpan.FromSeconds(2));

                return Result<CustomerCategory>.Success(request.AmountSpent >= 1000 ? CustomerCategory.Vip : CustomerCategory.Regular);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occured when registering customer.");
            }

            return Result<CustomerCategory>.Failure("Error occured when registering the customer.");

        }
    }
}