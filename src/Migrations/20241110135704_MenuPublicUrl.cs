using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteMenu.Migrations
{
    /// <inheritdoc />
    public partial class MenuPublicUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicUrl",
                table: "Menus",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "Menus",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_RestaurantId",
                table: "Menus",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Restaurants_RestaurantId",
                table: "Menus",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Restaurants_RestaurantId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Menus_RestaurantId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "PublicUrl",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Menus");
        }
    }
}
