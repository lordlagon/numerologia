namespace Numerologia.Core.Calculos;

// Tabela de Cores Favoráveis — pág. 220–221 do livro
// Baseada no Número de Expressão.
public static class TabelaCoresFavoraveis
{
    private static readonly Dictionary<int, string[]> _tabela = new()
    {
        [1] = ["Laranja", "Dourado", "Amarelo", "Branco"],
        [2] = ["Verde", "Crème", "Branco", "Preto"],
        [3] = ["Violeta", "Vinho", "Púrpura", "Vermelho"],
        [4] = ["Azul", "Cinza", "Púrpura", "Ouro"],
        [5] = ["Rosa", "Tons claros", "Cinza-rosa", "Pêssego"],
        [6] = ["Rosa", "Musgo", "Azul", "Verde"],
        [7] = ["Verde", "Amarelo", "Branco", "Cinza", "Azul-claro"],
        [8] = ["Púrpura", "Cítrus", "Azul", "Preto", "Castanho"],
        [9] = ["Vermelho", "Rosa", "Coral", "Vinho"]
    };

    public static string[] Consultar(int numeroExpressao)
    {
        if (!_tabela.TryGetValue(numeroExpressao, out var cores))
            throw new ArgumentOutOfRangeException(nameof(numeroExpressao),
                $"Número de Expressão '{numeroExpressao}' não consta na tabela. Use valores de 1 a 9.");

        return cores;
    }
}
