namespace Numerologia.Core.Calculos;

public class CalculoMapa
{
    private static readonly HashSet<int> _numerosComDivida = [13, 14, 16, 19];

    public ResultadoMapa Calcular(string nome)
    {
        var letras = nome
            .ToLowerInvariant()
            .Where(char.IsLetter)
            .ToList();

        var somaVogais     = letras.Where(TabelaCabalistica.EhVogal).Sum(TabelaCabalistica.ObterValor);
        var somaConsoantes = letras.Where(c => !TabelaCabalistica.EhVogal(c)).Sum(TabelaCabalistica.ObterValor);
        var somaTotal      = somaVogais + somaConsoantes;

        var figuraA            = CalcularFiguraA(letras);
        var licoesCarmicas     = CalcularLicoes(figuraA);
        var tendenciasOcultas  = CalcularTendencias(figuraA);

        return new ResultadoMapa(
            NumeroMotivacao:      ReducaoNumerologica.Reduzir(somaVogais),
            NumeroImpressao:      ReducaoNumerologica.Reduzir(somaConsoantes),
            NumeroExpressao:      ReducaoNumerologica.Reduzir(somaTotal),
            DividasCarmicas:      DetectarDividas(somaVogais, somaConsoantes, somaTotal),
            FiguraA:              figuraA,
            LicoesCarmicas:       licoesCarmicas,
            TendenciasOcultas:    tendenciasOcultas,
            RespostaSubconsciente: licoesCarmicas.Count
        );
    }

    private static IReadOnlyDictionary<int, int> CalcularFiguraA(List<char> letras)
    {
        var contagem = Enumerable.Range(1, 9).ToDictionary(n => n, _ => 0);

        foreach (var letra in letras)
        {
            var valor = TabelaCabalistica.ObterValor(letra);
            if (valor >= 1 && valor <= 9)
                contagem[valor]++;
        }

        return contagem;
    }

    private static IReadOnlyList<int> CalcularLicoes(IReadOnlyDictionary<int, int> figuraA) =>
        Enumerable.Range(1, 8).Where(n => figuraA[n] == 0).ToList();

    private static IReadOnlyList<int> CalcularTendencias(IReadOnlyDictionary<int, int> figuraA)
    {
        var maximo = figuraA.Values.Max();
        if (maximo == 0) return [];
        return figuraA.Where(kv => kv.Value == maximo).Select(kv => kv.Key).Order().ToList();
    }

    private static IReadOnlyList<int> DetectarDividas(int somaVogais, int somaConsoantes, int somaTotal)
    {
        var dividas = new[] { somaVogais, somaConsoantes, somaTotal }
            .Where(s => _numerosComDivida.Contains(s))
            .Distinct()
            .Order()
            .ToList();

        return dividas;
    }
}
