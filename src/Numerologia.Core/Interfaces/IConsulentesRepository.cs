using Numerologia.Core.Entities;

namespace Numerologia.Core.Interfaces;

public interface IConsulentesRepository
{
    Task<List<Consulente>> ObterTodosAsync(int usuarioId);
    Task<Consulente?> ObterPorIdAsync(int id, int usuarioId);
    Task AdicionarAsync(Consulente consulente);
    Task SalvarAlteracoesAsync();
    Task RemoverAsync(Consulente consulente);
}
