using System;
using System.Net.Http;
using System.Threading.Tasks;
using FunkyCustomerCare.Core;
using FunkyCustomerCare.DTO;
using Microsoft.Extensions.Logging;

namespace FunkyCustomerCare.Services
{
    public class CategorizeCustomerService : ICategorizeCustomerService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategorizeCustomerService> _logger;

        public CategorizeCustomerService(HttpClient httpClient, ILogger<CategorizeCustomerService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Result<CustomerCategory>> CategorizeAsync(CategorizeCustomerRequest request)
        {
            try
            {
                //
                // TODO: Implement the actual service where it categorizes the customer
                //

                await Task.Delay(TimeSpan.FromSeconds(2));

                return Result<CustomerCategory>.Success(request.AmountSpent >= 1000 ? CustomerCategory.Vip : CustomerCategory.Regular);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occured when categorizing customer.");
            }

            return Result<CustomerCategory>.Failure("Error occured when categorizing the customer.");

        }
    }
}