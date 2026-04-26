namespace Numerologia.Core.Calculos;

public class CalculoMapa
{
    // Pág. 117: valor final reduzido → dívida correspondente
    private static readonly Dictionary<int, int> _reducaoParaDivida = new()
    {
        { 4, 13 }, { 5, 14 }, { 7, 16 }, { 1, 19 }
    };

    public ResultadoMapa Calcular(string nome)
    {
        var grade = ConstruirGrade(nome);

        var letrasEfetivas = grade.Where(e => e.Tipo != TipoLetra.Espaco).ToList();

        var somaVogais     = letrasEfetivas.Where(e => e.Tipo == TipoLetra.Vogal).Sum(e => e.ValorCabalistico);
        var somaConsoantes = letrasEfetivas.Where(e => e.Tipo == TipoLetra.Consoante).Sum(e => e.ValorCabalistico);
        var somaTotal      = somaVogais + somaConsoantes;

        var motivacao = ReducaoNumerologica.Reduzir(somaVogais);
        var expressao = ReducaoNumerologica.Reduzir(somaTotal);
        var figuraA           = CalcularFiguraA(letrasEfetivas);
        var licoesCarmicas    = CalcularLicoes(figuraA);
        var tendenciasOcultas = CalcularTendencias(figuraA);

        return new ResultadoMapa(
            GradeLetras:          grade,
            NumeroMotivacao:      motivacao,
            NumeroImpressao:      ReducaoNumerologica.Reduzir(somaConsoantes),
            NumeroExpressao:      expressao,
            DividasCarmicas:      DetectarDividas(motivacao, expressao),
            FiguraA:              figuraA,
            LicoesCarmicas:       licoesCarmicas,
            TendenciasOcultas:    tendenciasOcultas,
            RespostaSubconsciente: 9 - licoesCarmicas.Count,
            RelacaoIntervalores:  CalcularRelacaoIntervalores(grade)
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
        // Pág. 114: Tendência Oculta existe apenas quando um número aparece MAIS de 3 vezes (≥ 4).
        // Se nenhum número atingir esse limiar → lista vazia.
        return figuraA
            .Where(kv => kv.Value >= 4)
            .Select(kv => kv.Key)
            .Order()
            .ToList();
    }

    // Pág. 203: RI = valor máximo entre as letras do primeiro nome (nome individual).
    // Regra: dobrar cada valor e o maior dobrado vence (equivalente a max).
    private static int CalcularRelacaoIntervalores(IReadOnlyList<EntradaLetra> grade)
    {
        var primeiroNome = grade.TakeWhile(e => e.Tipo != TipoLetra.Espaco).ToList();
        if (primeiroNome.Count == 0) return 0;
        return primeiroNome.Max(e => e.ValorCabalistico);
    }

    // Pág. 117: Motivação e Expressão com valor final 4/5/7/1 indicam Dívidas 13/14/16/19.
    // Dia de nascimento e Destino são verificados no GeradorMapa (requerem data).
    private static IReadOnlyList<int> DetectarDividas(int motivacao, int expressao)
    {
        var dividas = new HashSet<int>();

        if (_reducaoParaDivida.TryGetValue(motivacao, out var dMotivacao))
            dividas.Add(dMotivacao);
        if (_reducaoParaDivida.TryGetValue(expressao, out var dExpressao))
            dividas.Add(dExpressao);

        return dividas.Order().ToList();
    }
}
