using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Testcontainers.MsSql;
using TVMenukaart.Data;
using TVMenukaart.Interfaces;
using Xunit;

namespace TVMenukaart.IntegrationTests.Abstractions
{
    public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
            .Build();

        protected override void ConfigureClient(HttpClient client)
        {
            client.BaseAddress = new Uri("https://localhost");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<TVMenukaartContext>));

                services.AddDbContext<TVMenukaartContext>(options =>
                    options.UseSqlServer(_dbContainer.GetConnectionString()));
            });

            builder.ConfigureServices(services =>
            {
                // Remove original service
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBackgroundService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add mock
                var mock = new Mock<IBackgroundService>();
                mock.Setup(b => b.AddPhotoAsync(It.Is<IFormFile>(f => f.FileName.Equals("fail.jpeg"))))
                    .ReturnsAsync(new ImageUploadResult
                    {
                        Error = new Error
                        {
                            Message = "Error in background image service"
                        }
                    });

                mock.Setup(b => b.AddPhotoAsync(It.Is<IFormFile>(f => f.FileName.Equals("success.jpeg"))))
                    .ReturnsAsync(new ImageUploadResult
                    {
                        PublicId = "success",
                        SecureUrl = new Uri("https://www.thecodecrafter.nl")
                    });
                services.AddSingleton(mock.Object);
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
