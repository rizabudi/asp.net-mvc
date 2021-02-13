using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AdminLte.Data.Migrations.PostgreMigrations
{
    public partial class AddingEmployeeGroupModelToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupID",
                table: "Employees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmployeeGroup",
                columns: table => new
                {
                    GroupID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeGroup", x => x.GroupID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_GroupID",
                table: "Employees",
                column: "GroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_EmployeeGroup_GroupID",
                table: "Employees",
                column: "GroupID",
                principalTable: "EmployeeGroup",
                principalColumn: "GroupID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_EmployeeGroup_GroupID",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "EmployeeGroup");

            migrationBuilder.DropIndex(
                name: "IX_Employees_GroupID",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "GroupID",
                table: "Employees");
        }
    }
}
