using System.Threading.Tasks;
using FunkyCustomerCare.Core;
using FunkyCustomerCare.DTO;

namespace FunkyCustomerCare.Services
{
    public interface IRegisterCustomerService
    {
        Task<Result<CustomerCategory>> RegisterAsync(RegisterCustomerRequest request);
    }

    public enum CustomerCategory
    {
        Regular,
        Vip
    }
}