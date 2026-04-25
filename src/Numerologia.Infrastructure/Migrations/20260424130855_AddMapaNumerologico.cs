using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Numerologia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMapaNumerologico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mapas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConsulenteId = table.Column<int>(type: "integer", nullable: false),
                    NomeUtilizado = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DataNascimento = table.Column<DateOnly>(type: "date", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NumeroMotivacao = table.Column<int>(type: "integer", nullable: false),
                    NumeroImpressao = table.Column<int>(type: "integer", nullable: false),
                    NumeroExpressao = table.Column<int>(type: "integer", nullable: false),
                    DividasCarmicas = table.Column<string>(type: "text", nullable: false),
                    FiguraA = table.Column<string>(type: "text", nullable: false),
                    LicoesCarmicas = table.Column<string>(type: "text", nullable: false),
                    TendenciasOcultas = table.Column<string>(type: "text", nullable: false),
                    RespostaSubconsciente = table.Column<int>(type: "integer", nullable: false),
                    MesNascimentoReduzido = table.Column<int>(type: "integer", nullable: false),
                    DiaNascimentoReduzido = table.Column<int>(type: "integer", nullable: false),
                    AnoNascimentoReduzido = table.Column<int>(type: "integer", nullable: false),
                    NumeroDestino = table.Column<int>(type: "integer", nullable: false),
                    Missao = table.Column<int>(type: "integer", nullable: false),
                    CicloVida1 = table.Column<int>(type: "integer", nullable: false),
                    CicloVida2 = table.Column<int>(type: "integer", nullable: false),
                    CicloVida3 = table.Column<int>(type: "integer", nullable: false),
                    FimCiclo1Idade = table.Column<int>(type: "integer", nullable: false),
                    FimCiclo2Idade = table.Column<int>(type: "integer", nullable: false),
                    Desafio1 = table.Column<int>(type: "integer", nullable: false),
                    Desafio2 = table.Column<int>(type: "integer", nullable: false),
                    DesafioPrincipal = table.Column<int>(type: "integer", nullable: false),
                    MomentoDecisivo1 = table.Column<int>(type: "integer", nullable: false),
                    MomentoDecisivo2 = table.Column<int>(type: "integer", nullable: false),
                    MomentoDecisivo3 = table.Column<int>(type: "integer", nullable: false),
                    MomentoDecisivo4 = table.Column<int>(type: "integer", nullable: false),
                    DiasMesFavoraveis = table.Column<string>(type: "text", nullable: false),
                    NumerosHarmonicos = table.Column<string>(type: "text", nullable: false),
                    RelacaoIntervalores = table.Column<int>(type: "integer", nullable: false),
                    HarmoniaVibraCom = table.Column<int>(type: "integer", nullable: false),
                    HarmoniaAtrai = table.Column<string>(type: "text", nullable: false),
                    HarmoniaEOpostoA = table.Column<string>(type: "text", nullable: false),
                    HarmoniaProfundamenteOpostoA = table.Column<string>(type: "text", nullable: false),
                    HarmoniaEPassivoEm = table.Column<string>(type: "text", nullable: false),
                    CoresFavoraveis = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mapas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mapas_Consulentes_ConsulenteId",
                        column: x => x.ConsulenteId,
                        principalTable: "Consulentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mapas_ConsulenteId",
                table: "Mapas",
                column: "ConsulenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mapas");
        }
    }
}
