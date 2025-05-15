using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;
using TVMenukaart.Data;
using Xunit;

namespace TVMenukaart.UnitTests.Abstractions
{
    public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
            .Build();

        // protected override IHost CreateHost(IHostBuilder builder)
        // {
        //     // Add the TokenKey to config
        //     builder.ConfigureAppConfiguration(config =>
        //     {
        //         var testSettings = new Dictionary<string, string>
        //         {
        //             { "TokenKey", "2PGP8rFVFdHS3zeGiEB67a8ZWrxVIxrcxvqJV2WbOMsukQdXQ1jAqjuh3KYp9Fvb" }
        //         };
        //         config.AddInMemoryCollection(testSettings);
        //     });
        //     return base.CreateHost(builder);
        // }

        protected override void ConfigureClient(HttpClient client)
        {
            client.BaseAddress = new Uri("https://localhost");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // builder.ConfigureAppConfiguration((context, config) =>
            // {
            //     var testSettings = new Dictionary<string, string>
            //     {
            //         { "TokenKey", "2PGP8rFVFdHS3zeGiEB67a8ZWrxVIxrcxvqJV2WbOMsukQdXQ1jAqjuh3KYp9Fvb" }
            //     };
            //     config.AddInMemoryCollection(testSettings);
            // });

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<TVMenukaartContext>));

                services.AddDbContext<TVMenukaartContext>(options =>
                    options.UseSqlServer(_dbContainer.GetConnectionString()));
            });
        }

        public Task InitializeAsync()
        {
            return _dbContainer.StartAsync();
        }

        public Task DisposeAsync()
        {
            return _dbContainer.StopAsync();
        }
    }
}
