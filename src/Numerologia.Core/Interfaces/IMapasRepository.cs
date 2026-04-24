using Numerologia.Core.Entities;

namespace Numerologia.Core.Interfaces;

public interface IMapasRepository
{
    Task<List<MapaNumerologico>> ObterTodosAsync(int consulenteId, int usuarioId);
    Task<MapaNumerologico?> ObterPorIdAsync(int id, int consulenteId, int usuarioId);
    Task AdicionarAsync(MapaNumerologico mapa);
    Task SalvarAlteracoesAsync();
}
