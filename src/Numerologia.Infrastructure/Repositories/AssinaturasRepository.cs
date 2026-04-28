using Microsoft.EntityFrameworkCore;
using Numerologia.Core.Entities;
using Numerologia.Core.Interfaces;
using Numerologia.Infrastructure.Data;

namespace Numerologia.Infrastructure.Repositories;

public class AssinaturasRepository : IAssinaturasRepository
{
    private readonly AppDbContext _db;

    public AssinaturasRepository(AppDbContext db) => _db = db;

    public Task<List<AssinaturaTeste>> ObterTodosAsync(int mapaId) =>
        _db.AssinaturasTeste
           .Where(a => a.MapaId == mapaId)
           .OrderByDescending(a => a.CriadoEm)
           .ToListAsync();

    public Task<AssinaturaTeste?> ObterEscolhidaAsync(int mapaId) =>
        _db.AssinaturasTeste
           .Where(a => a.MapaId == mapaId && a.Escolhida)
           .FirstOrDefaultAsync();

    public async Task AdicionarAsync(AssinaturaTeste assinatura) =>
        await _db.AssinaturasTeste.AddAsync(assinatura);

    public async Task RemoverAsync(AssinaturaTeste assinatura) =>
        await Task.FromResult(_db.AssinaturasTeste.Remove(assinatura));

    public async Task EscolherAsync(int mapaId, int assinaturaId)
    {
        var todas = await _db.AssinaturasTeste
            .Where(a => a.MapaId == mapaId)
            .ToListAsync();

        foreach (var a in todas)
        {
            if (a.Id == assinaturaId) a.Escolher();
            else                      a.Desmarcar();
        }
    }

    public Task SalvarAlteracoesAsync() => _db.SaveChangesAsync();
}
