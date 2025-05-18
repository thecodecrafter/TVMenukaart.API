using Xunit;

namespace TVMenukaart.IntegrationTests.Abstractions
{
    [CollectionDefinition("FunctionalTests")]
    public class FunctionalTestCollection : ICollectionFixture<FunctionalTestWebAppFactory>
    {
    }
}
