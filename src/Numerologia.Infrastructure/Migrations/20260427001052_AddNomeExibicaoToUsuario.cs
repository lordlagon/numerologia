using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Numerologia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNomeExibicaoToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NomeExibicao",
                table: "Usuarios",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomeExibicao",
                table: "Usuarios");
        }
    }
}
