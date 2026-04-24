using Microsoft.EntityFrameworkCore;
using Numerologia.Core.Entities;
using Numerologia.Core.Interfaces;
using Numerologia.Infrastructure.Data;

namespace Numerologia.Infrastructure.Repositories;

public class ConsulentesRepository : IConsulentesRepository
{
    private readonly AppDbContext _context;

    public ConsulentesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Consulente>> ObterTodosAsync(int usuarioId) =>
        await _context.Consulentes
            .Where(c => c.UsuarioId == usuarioId)
            .OrderBy(c => c.NomeCompleto)
            .ToListAsync();

    public async Task<Consulente?> ObterPorIdAsync(int id, int usuarioId) =>
        await _context.Consulentes
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

    public async Task AdicionarAsync(Consulente consulente)
    {
        _context.Consulentes.Add(consulente);
        await _context.SaveChangesAsync();
    }

    public async Task SalvarAlteracoesAsync() =>
        await _context.SaveChangesAsync();

    public async Task RemoverAsync(Consulente consulente)
    {
        _context.Consulentes.Remove(consulente);
        await _context.SaveChangesAsync();
    }
}
