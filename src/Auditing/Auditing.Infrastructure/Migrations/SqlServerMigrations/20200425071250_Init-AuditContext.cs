using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Auditing.Infrastructure.Migrations.SqlServerMigrations
{
    public partial class InitAuditContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    TableName = table.Column<string>(maxLength: 30, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 30, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    OldValues = table.Column<string>(maxLength: 500, nullable: true),
                    NewValues = table.Column<string>(maxLength: 500, nullable: true),
                    ExtraData = table.Column<string>(maxLength: 500, nullable: true),
                    OperationType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tel = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 10, nullable: false),
                    Email = table.Column<string>(maxLength: 20, nullable: false),
                    Address = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
