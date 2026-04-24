namespace Numerologia.Core.Calculos;

// Tabela pág. 219 — "Números que se Harmonizam de Acordo com o Dia do Nascimento"
// Chave: dia de nascimento reduzido (1–9)
public static class TabelaNumerosHarmonicos
{
    private static readonly Dictionary<int, ResultadoNumerosHarmonicos> _tabela = new()
    {
        [1] = new(SeHarmonizamCom: [2, 4, 9],             NeutroCom: [1, 5, 6, 8]),
        [2] = new(SeHarmonizamCom: [1, 2, 3, 4, 5, 6, 7, 8, 9], NeutroCom: []),
        [3] = new(SeHarmonizamCom: [2, 3, 6, 8, 9],       NeutroCom: [4, 7]),
        [4] = new(SeHarmonizamCom: [1, 2, 7],              NeutroCom: [3, 5, 9]),
        [5] = new(SeHarmonizamCom: [2, 5, 6, 7, 9],       NeutroCom: [1, 4]),
        [6] = new(SeHarmonizamCom: [2, 3, 4, 5, 6, 9],    NeutroCom: [1]),
        [7] = new(SeHarmonizamCom: [2, 4, 5, 7],          NeutroCom: [3, 9]),
        [8] = new(SeHarmonizamCom: [2, 3, 9],              NeutroCom: [1, 6]),
        [9] = new(SeHarmonizamCom: [1, 2, 3, 5, 6, 8, 9], NeutroCom: [4, 7]),
    };

    public static ResultadoNumerosHarmonicos Consultar(int diaReduzido)
    {
        if (!_tabela.TryGetValue(diaReduzido, out var resultado))
            throw new ArgumentOutOfRangeException(nameof(diaReduzido),
                $"Dia reduzido '{diaReduzido}' não consta na tabela. Use valores de 1 a 9.");

        return resultado;
    }
}
