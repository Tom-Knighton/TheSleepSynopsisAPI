using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheSleepSynopsisAPI.Migrations
{
    public partial class DreamCreatedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Dreams",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Dreams",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Dreams");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Dreams");
        }
    }
}
