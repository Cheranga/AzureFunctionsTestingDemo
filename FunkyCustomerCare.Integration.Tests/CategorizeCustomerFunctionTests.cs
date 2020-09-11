using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FunkyCustomerCare.DTO;
using FunkyCustomerCare.Functions;
using FunkyCustomerCare.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using TestStack.BDDfy;
using Xunit;

namespace FunkyCustomerCare.Integration.Tests
{
    [Collection(IntegrationTestsCollection.Name)]
    public class CategorizeCustomerFunctionTests
    {
        public CategorizeCustomerFunctionTests(TestsInitializer testsInitializer)
        {
            _testsInitializer = testsInitializer;
        }

        private readonly TestsInitializer _testsInitializer;
        private CategorizeCustomerFunction _function;
        private CategorizeCustomerRequest _request;
        private HttpRequest _httpRequest;
        private IActionResult _response;

        private Task ThenMustReturnBadRequestResponse()
        {
            var badResponse = _response as BadRequestObjectResult;

            badResponse.Should().NotBeNull();
            badResponse.Value.Should().BeEquivalentTo("Invalid Request");

            return Task.CompletedTask;
        }

        private async Task WhenTheEndpointIsCalled()
        {
            var categorizeCustomerService = _testsInitializer.ServiceProvider.GetRequiredService<ICategorizeCustomerService>();
            var blobService = _testsInitializer.ServiceProvider.GetRequiredService<ICreateBlobService>();
            var validator = _testsInitializer.ServiceProvider.GetRequiredService<IValidator<CategorizeCustomerRequest>>();

            _function = new CategorizeCustomerFunction(categorizeCustomerService, blobService, validator, Mock.Of<ILogger<CategorizeCustomerFunction>>());

            _response = await _function.CreateAsync(_httpRequest);
        }

        private async Task GivenRequestIsInvalid(string fileName)
        {
            var content = await _testsInitializer.TestDataReader.GetFileContentAsync(fileName);
            _httpRequest = await GetMockedRequest(content);
        }

        private async Task<HttpRequest> GetMockedRequest(string data)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream) {AutoFlush = true};

            await streamWriter.WriteAsync(data);
            memoryStream.Position = 0;

            var mockedRequest = new Mock<HttpRequest>();
            mockedRequest.Setup(x => x.Body).Returns(memoryStream);

            return mockedRequest.Object;
        }

        [Theory]
        [InlineData("InvalidRequest_ValidJson")]
        [InlineData("InvalidRequest_Jibberish")]
        [InlineData("InvalidRequest_InvalidId")]
        [InlineData("InvalidRequest_InvalidAmountSpent")]
        public Task InvalidRequest(string fileName)
        {
            this.Given(x => GivenRequestIsInvalid(fileName))
                .When(x => WhenTheEndpointIsCalled())
                .Then(x => ThenMustReturnBadRequestResponse())
                .BDDfy();

            return Task.CompletedTask;
        }

        private async Task ThenMustCreateBlobInRegularContainer()
        {
            var content = await _testsInitializer.BlobService.GetContentAsync("regular-customers", _request.Id);

            content.Should().NotBeNullOrEmpty();

            var uploadedData = JsonConvert.DeserializeObject<CategorizeCustomerRequest>(content);
            uploadedData.Should().NotBeNull();
        }

        private async Task GivenRequestIsForRegularCustomer()
        {
            _request = _testsInitializer.Fixture.Create<CategorizeCustomerRequest>();
            _request.AmountSpent = 100;

            _httpRequest = await GetMockedRequest(JsonConvert.SerializeObject(_request));
        }

        private async Task ThenMustCreateBlobInVipContainer()
        {
            var content = await _testsInitializer.BlobService.GetContentAsync("vip-customers", _request.Id);

            content.Should().NotBeNullOrEmpty();

            var uploadedData = JsonConvert.DeserializeObject<CategorizeCustomerRequest>(content);
            uploadedData.Should().NotBeNull();
        }

        private Task ThenMustReturnAcceptedResponse()
        {
            var actionResult = _response as AcceptedResult;
            actionResult.Should().NotBeNull();

            return Task.CompletedTask;
        }

        private async Task GivenRequestIsForVipCustomer()
        {
            _request = _testsInitializer.Fixture.Create<CategorizeCustomerRequest>();
            _request.AmountSpent = 1200;

            _httpRequest = await GetMockedRequest(JsonConvert.SerializeObject(_request));
        }

        [Fact]
        public Task RegularCustomers()
        {
            this.Given(x => GivenRequestIsForRegularCustomer())
                .When(x => WhenTheEndpointIsCalled())
                .Then(x => ThenMustReturnAcceptedResponse())
                .And(x => ThenMustCreateBlobInRegularContainer())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Fact]
        public Task VipCustomers()
        {
            this.Given(x => GivenRequestIsForVipCustomer())
                .When(x => WhenTheEndpointIsCalled())
                .Then(x => ThenMustReturnAcceptedResponse())
                .And(x => ThenMustCreateBlobInVipContainer())
                .BDDfy();

            return Task.CompletedTask;
        }
    }
}