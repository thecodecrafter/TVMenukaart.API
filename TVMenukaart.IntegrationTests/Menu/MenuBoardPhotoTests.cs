using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.Menu
{
    public class MenuBoardPhotoTests : BaseFunctionalTest
    {
        public MenuBoardPhotoTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_ReturnBadRequest_WhenBackgroundImageServiceFails()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var testMenu = DataSeeder.GetTestMenu();
            var client = await CreateClientWithAuth(appUser.UserName);

            // Act
            var response = await client.PostAsync($"api/menu/{testMenu.Id}/board-photo", CreateFormData(false));
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Be("Error in background image service");
        }

        [Fact]
        public async Task Should_ReturnNoContent_WhenIsSuccessfull()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var testMenu = DataSeeder.GetTestMenu();
            var client = await CreateClientWithAuth(appUser.UserName);

            // Act
            var response = await client.PostAsync($"api/menu/{testMenu.Id}/board-photo", CreateFormData());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenMenuIsNotFound()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);

            // Act
            var response = await client.PostAsync("api/menu/9999/board-photo", CreateFormData());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private MultipartFormDataContent CreateFormData(bool success = true)
        {
            var imageBytes = GenerateImageByteArray();
            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            var formData = new MultipartFormDataContent();
            formData.Add(imageContent, "file", success ? "success.jpeg" : "fail.jpeg");

            return formData;
        }

        private byte[] GenerateImageByteArray()
        {
            // This is a minimal valid JPEG file (a tiny black 1x1 pixel image)
            return new byte[]
            {
                0xFF, 0xD8, // SOI (Start of Image)
                0xFF, 0xE0, // APP0 marker
                0x00, 0x10, // APP0 block length
                0x4A, 0x46, 0x49, 0x46, 0x00, // "JFIF\0"
                0x01, 0x01, // version
                0x00, // units
                0x00, 0x01, 0x00, 0x01, // density
                0x00, 0x00, // thumbnail width & height
                0xFF, 0xDB, 0x00, 0x43, 0x00, // quantization table
                // ... (you can skip full table for brevity)
                0xFF, 0xC0, 0x00, 0x11, // Start of Frame marker
                0x08, 0x00, 0x01, 0x00, 0x01, 0x03, 0x01, 0x11, 0x00,
                0x02, 0x11, 0x01, 0x03, 0x11, 0x01,
                0xFF, 0xDA, 0x00, 0x0C, // Start of Scan marker
                0x03, 0x01, 0x00, 0x02, 0x11, 0x03, 0x11,
                0x00, 0x3F, 0x00, 0xD2, 0xCF, 0x20,
                0xFF, 0xD9 // EOI (End of Image)
            };
        }
    }
}
