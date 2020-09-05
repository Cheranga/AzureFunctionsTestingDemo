using System.Threading.Tasks;
using FunkyCustomerCare.Core;
using FunkyCustomerCare.DTO;

namespace FunkyCustomerCare.Services
{
    public interface ICategorizeCustomerService
    {
        Task<Result<CustomerCategory>> CategorizeAsync(CategorizeCustomerRequest request);
    }
}