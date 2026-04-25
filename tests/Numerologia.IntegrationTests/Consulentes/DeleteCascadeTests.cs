using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Numerologia.Core.Entities;
using Numerologia.Infrastructure.Data;
using Numerologia.IntegrationTests.Infraestrutura;

namespace Numerologia.IntegrationTests.Consulentes;

public class DeleteCascadeTests : IClassFixture<NumerologiaWebFactory>
{
    private readonly NumerologiaWebFactory _factory;
    private const string GoogleId = "google-cascade-test";
    private const string Email    = "cascade@test.com";
    private const string Nome     = "Numeróloga Cascade";

    public DeleteCascadeTests(NumerologiaWebFactory factory)
    {
        _factory = factory;

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (!db.Usuarios.Any(u => u.GoogleId == GoogleId))
        {
            db.Usuarios.Add(new Usuario(GoogleId, Email, Nome));
            db.SaveChanges();
        }
    }

    private HttpClient AuthClient() => _factory.CreateAuthenticatedClient(GoogleId, Email, Nome);

    [Fact]
    public async Task Delete_ConsulentComMapas_Retorna204_EMapasSaoExcluidos()
    {
        var client = AuthClient();

        // Cria consulente via API
        var criarConsulente = await client.PostAsJsonAsync("/api/consulentes", new
        {
            NomeCompleto   = "Consulente Cascade Delete",
            DataNascimento = "1985-03-10"
        });
        criarConsulente.StatusCode.Should().Be(HttpStatusCode.Created);
        var consulente = await criarConsulente.Content.ReadFromJsonAsync<ConsulenteResponse>();

        // Cria mapa para o consulente
        var criarMapa = await client.PostAsJsonAsync(
            $"/api/consulentes/{consulente!.Id}/mapas",
            new { NomeUtilizado = "CONSULENTE CASCADE" });
        criarMapa.StatusCode.Should().Be(HttpStatusCode.Created);
        var mapa = await criarMapa.Content.ReadFromJsonAsync<MapaResumoResponse>();

        // Verifica que o mapa existe no banco antes de excluir
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Mapas.Any(m => m.Id == mapa!.Id).Should().BeTrue();
        }

        // Exclui o consulente
        var deleteResponse = await client.DeleteAsync($"/api/consulentes/{consulente.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verifica que o mapa foi excluído em cascata
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Mapas.Any(m => m.Id == mapa!.Id).Should().BeFalse();
            db.Consulentes.Any(c => c.Id == consulente.Id).Should().BeFalse();
        }
    }

    [Fact]
    public void Delete_Usuario_RemoveConsulentesEMapasEmCascata()
    {
        // Cria usuário, consulente e mapa diretamente no banco (sem API) para isolar o teste de cascade
        const string googleIdLocal = "google-cascade-usuario";
        int usuarioId, consulenteId, mapaId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db      = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var gerador = scope.ServiceProvider.GetRequiredService<Numerologia.Core.Services.GeradorMapa>();

            if (!db.Usuarios.Any(u => u.GoogleId == googleIdLocal))
            {
                db.Usuarios.Add(new Usuario(googleIdLocal, "cascade-user@test.com", "Cascade User"));
                db.SaveChanges();
            }

            var usuario = db.Usuarios.First(u => u.GoogleId == googleIdLocal);
            usuarioId = usuario.Id;

            var consulente = new Consulente(usuarioId, "Jose Da Silva", new DateOnly(1985, 3, 10));
            db.Consulentes.Add(consulente);
            db.SaveChanges();
            consulenteId = consulente.Id;

            var mapa = gerador.Gerar(consulenteId, "JOSE DA SILVA", new DateOnly(1985, 3, 10));
            db.Mapas.Add(mapa);
            db.SaveChanges();
            mapaId = mapa.Id;
        }

        // Confirma que consulente e mapa existem antes de excluir
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Consulentes.Any(c => c.Id == consulenteId).Should().BeTrue();
            db.Mapas.Any(m => m.Id == mapaId).Should().BeTrue();
        }

        // Remove o usuário diretamente do banco (simula exclusão de conta)
        using (var scope = _factory.Services.CreateScope())
        {
            var db      = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var usuario = db.Usuarios.Find(usuarioId);
            db.Usuarios.Remove(usuario!);
            db.SaveChanges();
        }

        // Verifica que consulente e mapa foram excluídos em cascata
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Consulentes.Any(c => c.Id == consulenteId).Should().BeFalse();
            db.Mapas.Any(m => m.Id == mapaId).Should().BeFalse();
        }
    }

    private record ConsulenteResponse(int Id, string NomeCompleto, DateOnly DataNascimento,
        string? Email, string? Telefone, DateTime CriadoEm);

    private record MapaResumoResponse(int Id, string NomeUtilizado, DateOnly DataNascimento,
        int NumeroMotivacao, int NumeroImpressao, int NumeroExpressao, int NumeroDestino, DateTime CriadoEm);
}
