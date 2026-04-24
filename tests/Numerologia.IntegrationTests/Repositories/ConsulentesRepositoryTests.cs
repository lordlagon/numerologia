using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Numerologia.Core.Entities;
using Numerologia.Infrastructure.Data;
using Numerologia.Infrastructure.Repositories;

namespace Numerologia.IntegrationTests.Repositories;

public class ConsulentesRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly ConsulentesRepository _sut;
    private readonly int _usuarioId;

    public ConsulentesRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        _sut = new ConsulentesRepository(_context);

        // Usuário de base para os testes
        var usuario = new Usuario("google-repo-test", "repo@test.com", "Repo User");
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
        _usuarioId = usuario.Id;
    }

    [Fact]
    public async Task AdicionarAsync_DevePersistirConsulente()
    {
        var consulente = new Consulente(_usuarioId, "Ana Clara", new DateOnly(1992, 4, 10));

        await _sut.AdicionarAsync(consulente);

        var salvo = await _context.Consulentes.FirstOrDefaultAsync(c => c.NomeCompleto == "Ana Clara");
        salvo.Should().NotBeNull();
        salvo!.UsuarioId.Should().Be(_usuarioId);
        salvo.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ObterTodosAsync_RetornaApenasConsulentesDoUsuario()
    {
        var outroUsuario = new Usuario("google-outro", "outro@test.com", "Outro");
        _context.Usuarios.Add(outroUsuario);
        await _context.SaveChangesAsync();

        _context.Consulentes.AddRange(
            new Consulente(_usuarioId,       "Consulente A", new DateOnly(1990, 1, 1)),
            new Consulente(_usuarioId,       "Consulente B", new DateOnly(1991, 2, 2)),
            new Consulente(outroUsuario.Id,  "Consulente C", new DateOnly(1992, 3, 3)));
        await _context.SaveChangesAsync();

        var resultado = await _sut.ObterTodosAsync(_usuarioId);

        resultado.Should().HaveCount(2);
        resultado.Select(c => c.NomeCompleto).Should().BeEquivalentTo(["Consulente A", "Consulente B"]);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoExisteEPertenceAoUsuario_Retorna()
    {
        var consulente = new Consulente(_usuarioId, "Bruno", new DateOnly(1988, 5, 15));
        _context.Consulentes.Add(consulente);
        await _context.SaveChangesAsync();

        var resultado = await _sut.ObterPorIdAsync(consulente.Id, _usuarioId);

        resultado.Should().NotBeNull();
        resultado!.NomeCompleto.Should().Be("Bruno");
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoPertenceAOutroUsuario_RetornaNull()
    {
        var outroUsuario = new Usuario("google-isolado", "isolado@test.com", "Isolado");
        _context.Usuarios.Add(outroUsuario);
        await _context.SaveChangesAsync(); // salvar antes para obter o Id gerado

        var consulente = new Consulente(outroUsuario.Id, "Consulente Alheio", new DateOnly(1990, 1, 1));
        _context.Consulentes.Add(consulente);
        await _context.SaveChangesAsync();

        var resultado = await _sut.ObterPorIdAsync(consulente.Id, _usuarioId);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task RemoverAsync_DeveDeletarConsulente()
    {
        var consulente = new Consulente(_usuarioId, "Carlos", new DateOnly(1995, 8, 20));
        _context.Consulentes.Add(consulente);
        await _context.SaveChangesAsync();

        await _sut.RemoverAsync(consulente);

        var deletado = await _context.Consulentes.FindAsync(consulente.Id);
        deletado.Should().BeNull();
    }

    [Fact]
    public async Task SalvarAlteracoesAsync_PersisteMudancas()
    {
        var consulente = new Consulente(_usuarioId, "Diana", new DateOnly(1987, 11, 3));
        _context.Consulentes.Add(consulente);
        await _context.SaveChangesAsync();

        consulente.Atualizar("Diana Atualizada", new DateOnly(1987, 11, 3), null, null);
        await _sut.SalvarAlteracoesAsync();

        var atualizado = await _context.Consulentes.FindAsync(consulente.Id);
        atualizado!.NomeCompleto.Should().Be("Diana Atualizada");
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
