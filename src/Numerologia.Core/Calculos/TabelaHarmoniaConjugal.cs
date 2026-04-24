namespace Numerologia.Core.Calculos;

// Tabela de Harmonia Conjugal — pág. 212 do livro
// Baseada no Número de Expressão de cada pessoa.
public static class TabelaHarmoniaConjugal
{
    private static readonly Dictionary<int, ResultadoHarmoniaConjugal> _tabela = new()
    {
        [1] = new(VibraCom: 9, Atrai: [4, 8],      EOpostoA: [6, 7], ProfundamenteOpostoA: [],  EPassivoEm: [2, 3, 5]),
        [2] = new(VibraCom: 8, Atrai: [7, 9],      EOpostoA: [5],    ProfundamenteOpostoA: [],  EPassivoEm: [1, 3, 4, 6]),
        [3] = new(VibraCom: 7, Atrai: [5, 6, 9],   EOpostoA: [4, 8], ProfundamenteOpostoA: [],  EPassivoEm: [1, 2]),
        [4] = new(VibraCom: 6, Atrai: [1, 8],      EOpostoA: [3, 5], ProfundamenteOpostoA: [],  EPassivoEm: [2, 7, 9]),
        [5] = new(VibraCom: 5, Atrai: [3, 9],      EOpostoA: [2, 4], ProfundamenteOpostoA: [6], EPassivoEm: [1, 7, 8]),
        [6] = new(VibraCom: 4, Atrai: [3, 7, 9],   EOpostoA: [1, 8], ProfundamenteOpostoA: [5], EPassivoEm: [2]),
        [7] = new(VibraCom: 3, Atrai: [2, 6],      EOpostoA: [1, 9], ProfundamenteOpostoA: [],  EPassivoEm: [4, 5, 8]),
        [8] = new(VibraCom: 2, Atrai: [1, 4],      EOpostoA: [3, 6], ProfundamenteOpostoA: [],  EPassivoEm: [5, 7, 9]),
        [9] = new(VibraCom: 1, Atrai: [2, 3, 5, 6], EOpostoA: [7],  ProfundamenteOpostoA: [],  EPassivoEm: [4, 8])
    };

    public static ResultadoHarmoniaConjugal Consultar(int numeroExpressao)
    {
        if (!_tabela.TryGetValue(numeroExpressao, out var resultado))
            throw new ArgumentOutOfRangeException(nameof(numeroExpressao),
                $"Número de Expressão '{numeroExpressao}' não consta na tabela. Use valores de 1 a 9.");

        return resultado;
    }
}
