using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TVMenukaart.Data;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.TestData;
using TVMenukaart.Models;
using Xunit;

namespace TVMenukaart.IntegrationTests.Abstractions
{
    [Collection("FunctionalTests")]
    public abstract class BaseFunctionalTest : IAsyncLifetime
    {
        private readonly IServiceScope _scope;
        protected readonly FunctionalTestWebAppFactory _factory;
        protected readonly TVMenukaartContext DbContext;
        protected readonly UserManager<AppUser> UserManager;
        protected readonly TestDataSeeder DataSeeder;
        
        protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
        {
            _factory = factory;
            _scope = _factory.Services.CreateScope();
            
            DbContext = _scope.ServiceProvider.GetRequiredService<TVMenukaartContext>();
            UserManager = _scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            DataSeeder = new TestDataSeeder(DbContext, UserManager);
        }

        protected async Task<HttpClient> CreateClientWithAuth(string username = "authenticatedUser")
        {
            var loginRequest = new LoginDto { Username = username, Password = "Pa$$w0rd" };
            var loginResponse = await HttpClient.PostAsJsonAsync("api/Account/login", loginRequest);
            
            if (loginResponse.StatusCode == HttpStatusCode.NotFound)
            {
                var registerRequest = new RegisterDto { Username = username, Password = "Pa$$w0rd" };
                await HttpClient.PostAsJsonAsync("api/Account/register", registerRequest);    
            }
            
            loginResponse = await HttpClient.PostAsJsonAsync("api/Account/login", loginRequest);
            loginResponse.EnsureSuccessStatusCode();
            var userDto = await loginResponse.Content.ReadFromJsonAsync<UserDto>();
            
            var client = _factory.CreateDefaultClient();
            if (userDto != null)
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, userDto.Token);
            }

            return client;
        }

        protected HttpClient HttpClient => _factory.CreateDefaultClient();
        
        public Task InitializeAsync()
        {
            return DataSeeder.SeedAsync();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
