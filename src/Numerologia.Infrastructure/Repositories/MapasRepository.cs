using Microsoft.EntityFrameworkCore;
using Numerologia.Core.Entities;
using Numerologia.Core.Interfaces;
using Numerologia.Infrastructure.Data;

namespace Numerologia.Infrastructure.Repositories;

public class MapasRepository : IMapasRepository
{
    private readonly AppDbContext _db;

    public MapasRepository(AppDbContext db) => _db = db;

    public async Task<List<MapaNumerologico>> ObterTodosAsync(int consulenteId, int usuarioId)
    {
        var consulenteExiste = await _db.Consulentes
            .AnyAsync(c => c.Id == consulenteId && c.UsuarioId == usuarioId);

        if (!consulenteExiste)
            return null!; // sinaliza 404 para o endpoint

        return await _db.Mapas
            .Where(m => m.ConsulenteId == consulenteId)
            .OrderByDescending(m => m.CriadoEm)
            .ToListAsync();
    }

    public async Task<MapaNumerologico?> ObterPorIdAsync(int id, int consulenteId, int usuarioId)
    {
        var consulenteExiste = await _db.Consulentes
            .AnyAsync(c => c.Id == consulenteId && c.UsuarioId == usuarioId);

        if (!consulenteExiste) return null;

        return await _db.Mapas
            .FirstOrDefaultAsync(m => m.Id == id && m.ConsulenteId == consulenteId);
    }

    public async Task AdicionarAsync(MapaNumerologico mapa)
        => await _db.Mapas.AddAsync(mapa);

    public Task RemoverAsync(MapaNumerologico mapa)
    {
        _db.Mapas.Remove(mapa);
        return Task.CompletedTask;
    }

    public async Task SalvarAlteracoesAsync()
        => await _db.SaveChangesAsync();
}
