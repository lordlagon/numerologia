using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Numerologia.Core.Entities;
using Numerologia.Infrastructure.Data;
using Numerologia.Infrastructure.Repositories;

namespace Numerologia.IntegrationTests.Repositories;

public class UsuarioRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly UsuarioRepository _sut;

    public UsuarioRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        _sut = new UsuarioRepository(_context);
    }

    [Fact]
    public async Task AdicionarAsync_DevePeristirUsuarioNoBanco()
    {
        var usuario = new Usuario("google-123", "joao@gmail.com", "João");

        await _sut.AdicionarAsync(usuario);

        var salvo = await _context.Usuarios.FirstOrDefaultAsync(u => u.GoogleId == "google-123");
        salvo.Should().NotBeNull();
        salvo!.Email.Should().Be("joao@gmail.com");
        salvo.Nome.Should().Be("João");
        salvo.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ObterPorGoogleIdAsync_QuandoExiste_DeveRetornarUsuario()
    {
        var usuario = new Usuario("google-456", "maria@gmail.com", "Maria");
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var resultado = await _sut.ObterPorGoogleIdAsync("google-456");

        resultado.Should().NotBeNull();
        resultado!.Email.Should().Be("maria@gmail.com");
    }

    [Fact]
    public async Task ObterPorGoogleIdAsync_QuandoNaoExiste_DeveRetornarNull()
    {
        var resultado = await _sut.ObterPorGoogleIdAsync("inexistente");

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task GoogleId_DeveSerUnico()
    {
        var usuario1 = new Usuario("google-dup", "a@test.com", "A");
        _context.Usuarios.Add(usuario1);
        await _context.SaveChangesAsync();

        var usuario2 = new Usuario("google-dup", "b@test.com", "B");
        _context.Usuarios.Add(usuario2);

        var act = () => _context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
