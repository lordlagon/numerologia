namespace Numerologia.Core.Calculos;

public class CalculoMapa
{
    private static readonly HashSet<int> _numerosComDivida = [13, 14, 16, 19];

    public ResultadoMapa Calcular(string nome)
    {
        var grade = ConstruirGrade(nome);

        var letrasEfetivas = grade.Where(e => e.Tipo != TipoLetra.Espaco).ToList();

        var somaVogais     = letrasEfetivas.Where(e => e.Tipo == TipoLetra.Vogal).Sum(e => e.ValorCabalistico);
        var somaConsoantes = letrasEfetivas.Where(e => e.Tipo == TipoLetra.Consoante).Sum(e => e.ValorCabalistico);
        var somaTotal      = somaVogais + somaConsoantes;

        var figuraA           = CalcularFiguraA(letrasEfetivas);
        var licoesCarmicas    = CalcularLicoes(figuraA);
        var tendenciasOcultas = CalcularTendencias(figuraA);

        return new ResultadoMapa(
            GradeLetras:          grade,
            NumeroMotivacao:      ReducaoNumerologica.Reduzir(somaVogais),
            NumeroImpressao:      ReducaoNumerologica.Reduzir(somaConsoantes),
            NumeroExpressao:      ReducaoNumerologica.Reduzir(somaTotal),
            DividasCarmicas:      DetectarDividas(somaVogais, somaConsoantes, somaTotal),
            FiguraA:              figuraA,
            LicoesCarmicas:       licoesCarmicas,
            TendenciasOcultas:    tendenciasOcultas,
            RespostaSubconsciente: 9 - licoesCarmicas.Count
        );
    }

    private static IReadOnlyList<EntradaLetra> ConstruirGrade(string nome)
    {
        var grade = new List<EntradaLetra>();

        foreach (var ch in nome.ToUpperInvariant())
        {
            if (ch == ' ')
            {
                grade.Add(new EntradaLetra(' ', TipoLetra.Espaco, 0));
                continue;
            }

            if (!char.IsLetter(ch)) continue;

            var lower = char.ToLowerInvariant(ch);
            var valor = TabelaCabalistica.ObterValor(lower);
            var tipo  = TabelaCabalistica.EhVogal(lower) ? TipoLetra.Vogal : TipoLetra.Consoante;

            grade.Add(new EntradaLetra(ch, tipo, valor));
        }

        return grade;
    }

    private static IReadOnlyDictionary<int, int> CalcularFiguraA(List<EntradaLetra> letras)
    {
        var contagem = Enumerable.Range(1, 9).ToDictionary(n => n, _ => 0);

        foreach (var entrada in letras)
        {
            var valor = entrada.ValorCabalistico;
            if (valor >= 1 && valor <= 9)
                contagem[valor]++;
        }

        return contagem;
    }

    private static IReadOnlyList<int> CalcularLicoes(IReadOnlyDictionary<int, int> figuraA) =>
        Enumerable.Range(1, 9).Where(n => figuraA[n] == 0).ToList();

    private static IReadOnlyList<int> CalcularTendencias(IReadOnlyDictionary<int, int> figuraA)
    {
        var maximo = figuraA.Values.Max();
        if (maximo == 0) return [];
        return figuraA.Where(kv => kv.Value == maximo).Select(kv => kv.Key).Order().ToList();
    }

    private static IReadOnlyList<int> DetectarDividas(int somaVogais, int somaConsoantes, int somaTotal)
    {
        return new[] { somaVogais, somaConsoantes, somaTotal }
            .Where(s => _numerosComDivida.Contains(s))
            .Distinct()
            .Order()
            .ToList();
    }
}
