using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using TVMenukaart.IntegrationTests.Abstractions;
using TVMenukaart.Interfaces;
using Xunit;

namespace TVMenukaart.IntegrationTests.Menu
{
    public class MenuBoardPhotoTests : BaseFunctionalTest
    {
        private readonly MockRepository _mockRepository = new(MockBehavior.Strict);
        private readonly Mock<IBackgroundService> _mockBackgroundImageService;

        public MenuBoardPhotoTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _mockBackgroundImageService = _mockRepository.Create<IBackgroundService>();
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenMenuIsNotFound()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            
            var imageStream = new MemoryStream(GenerateImageByteArray());
            var image = new FormFile(imageStream, 0, imageStream.Length, "UnitTest", "UnitTest.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            // Act
            var response = await client.PostAsJsonAsync("api/menu/9999/board-photo", image);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_ReturnBadRequest_WhenMenuIsNotFound()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);

            // Act
            var response = await client.PostAsJsonAsync("api/menu/9999/board-photo", new { });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private byte[] GenerateImageByteArray(int width = 50, int height = 50)
        {
            // Bitmap bitmapImage = new Bitmap(width, height);
            // Graphics imageData = Graphics.FromImage(bitmapImage);
            // imageData.DrawLine(new Pen(Color.Blue), 0, 0, width, height);

            MemoryStream memoryStream = new MemoryStream();
            byte[] byteArray;

            using (memoryStream)
            {
                byteArray = memoryStream.ToArray();
            }
            return byteArray;
        }
    }
}
