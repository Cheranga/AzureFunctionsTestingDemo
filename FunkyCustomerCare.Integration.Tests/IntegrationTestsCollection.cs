using Xunit;

namespace FunkyCustomerCare.Integration.Tests
{
    [CollectionDefinition(Name)]
    public class IntegrationTestsCollection : ICollectionFixture<TestsInitializer>
    {
        public const string Name = nameof(IntegrationTestsCollection);
    }
}