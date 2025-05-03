using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteMenu.Migrations
{
    /// <inheritdoc />
    public partial class DeviceCodeFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceCodes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "TEXT", nullable: true),
                    PollingToken = table.Column<string>(type: "TEXT", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpirationInSeconds = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceCodes");
        }
    }
}
