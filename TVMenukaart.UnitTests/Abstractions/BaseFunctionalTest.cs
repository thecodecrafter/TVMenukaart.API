using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace TVMenukaart.UnitTests.Abstractions
{
    public class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
    {
        protected readonly FunctionalTestWebAppFactory _factory;

        public BaseFunctionalTest(FunctionalTestWebAppFactory factory)
        {
            _factory = factory;
        }

        protected HttpClient CreateClient(string token, CookieContainer cookies = null)
        {
            cookies ??= new CookieContainer();

            // var client = _factory.CreateDefaultClient(new CookieDelegatingHandler(cookies));
            var client = _factory.CreateDefaultClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        protected HttpClient HttpClient => _factory.CreateDefaultClient();
    }
}
