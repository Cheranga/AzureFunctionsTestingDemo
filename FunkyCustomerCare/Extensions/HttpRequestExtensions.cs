using System;
using System.IO;
using System.Threading.Tasks;
using FunkyCustomerCare.Core;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FunkyCustomerCare.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task<Result<TModel>> GetModel<TModel>(this HttpRequest request) where TModel: class
        {
            try
            {
                var content = await new StreamReader(request.Body).ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(content))
                {
                    return Result<TModel>.Failure("Empty request.");
                }

                var model = JsonConvert.DeserializeObject<TModel>(content, new JsonSerializerSettings
                {
                    Error = (sender, args) => args.ErrorContext.Handled = true
                });

                if (model == null)
                {
                    return Result<TModel>.Failure("Cannot deserialize the object to the required type.");
                }

                return Result<TModel>.Success(model);
            }
            catch (Exception exception)
            {
                // ignore
            }

            return Result<TModel>.Failure("Error occured when converting the request content to the desired type.");
        }
    }
}