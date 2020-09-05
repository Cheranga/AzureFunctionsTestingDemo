using System.Threading.Tasks;
using FunkyCustomerCare.Core;
using FunkyCustomerCare.Functions;
using FunkyCustomerCare.Models;
using Microsoft.Azure.WebJobs;

namespace FunkyCustomerCare.Services
{
    public interface ICreateBlobService
    {
        Task<Result> CreateBlobAsync(IBinder binder, CreateBlobRequest request);
    }
}