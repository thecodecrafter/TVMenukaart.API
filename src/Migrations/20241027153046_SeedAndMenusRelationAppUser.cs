using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteMenu.Migrations
{
    /// <inheritdoc />
    public partial class SeedAndMenusRelationAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Menus",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_AppUserId",
                table: "Menus",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_AspNetUsers_AppUserId",
                table: "Menus",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menus_AspNetUsers_AppUserId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Menus_AppUserId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Menus");
        }
    }
}
