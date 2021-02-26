using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AdminLte.Data.Migrations.PostgreMigrations
{
    public partial class UpdateQuestionDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRandom",
                table: "Questions");

            migrationBuilder.AddColumn<bool>(
                name: "IsUnFavorable",
                table: "QuestionAnswer",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUnFavorable",
                table: "QuestionAnswer");

            migrationBuilder.AddColumn<bool>(
                name: "IsRandom",
                table: "Questions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
