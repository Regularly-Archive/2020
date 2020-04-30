using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Auditing.Infrastructure.Migrations.SqlServerMigrations
{
    public partial class AddFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BusinessUnit",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "BusinessUnit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BusinessUnit");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "BusinessUnit");
        }
    }
}
