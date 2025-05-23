using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.MenuSections
{
    public class UpdateMenuSectionsTests : BaseFunctionalTest
    {
        public UpdateMenuSectionsTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenMenuDoesNotExist()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);

            // Act
            var response = await client.PatchAsJsonAsync($"api/menusection?menuId={99999}&menuSectionId=88888&sectionName=test",
                new { });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenMenuSectionDoesNotExist()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var menu = DataSeeder.GetTestMenu();

            // Act
            var response = await client.PatchAsJsonAsync($"api/menusection?menuId={menu.Id}&menuSectionId=88888&sectionName=test",
                new { });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task Should_ReturnOk_WhenMenuSectionIsUpdated()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var menu = DataSeeder.GetTestMenu();
            var menuSection = menu.MenuSections.FirstOrDefault();

            if (menuSection == null)
            {
                menuSection = new Models.MenuSection("New TestMenuSection");
                menu.MenuSections.Add(menuSection);
                await DbContext.SaveChangesAsync();
            }

            // Act
            var response = await client.PatchAsJsonAsync($"api/menusection?menuId={menu.Id}&menuSectionId={menuSection.Id}&sectionName=test",
                new { });
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
