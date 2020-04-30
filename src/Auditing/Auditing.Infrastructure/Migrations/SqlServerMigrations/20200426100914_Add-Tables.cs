using Microsoft.EntityFrameworkCore.Migrations;

namespace Auditing.Infrastructure.Migrations.SqlServerMigrations
{
    public partial class AddTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessUnit",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    OrgName = table.Column<string>(nullable: true),
                    OrgCode = table.Column<string>(nullable: true),
                    IsActive = table.Column<string>(nullable: true),
                    ParentOrg = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessUnit", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessUnit");
        }
    }
}
