using Numerologia.Core.Entities;

namespace Numerologia.Core.Interfaces;

public interface IAssinaturasRepository
{
    Task<List<AssinaturaTeste>> ObterTodosAsync(int mapaId);
    Task<AssinaturaTeste?> ObterEscolhidaAsync(int mapaId);
    Task AdicionarAsync(AssinaturaTeste assinatura);
    Task RemoverAsync(AssinaturaTeste assinatura);
    Task EscolherAsync(int mapaId, int assinaturaId);
    Task SalvarAlteracoesAsync();
}
