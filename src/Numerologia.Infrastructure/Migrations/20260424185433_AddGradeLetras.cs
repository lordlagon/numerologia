using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Numerologia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGradeLetras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GradeLetras",
                table: "Mapas",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GradeLetras",
                table: "Mapas");
        }
    }
}
