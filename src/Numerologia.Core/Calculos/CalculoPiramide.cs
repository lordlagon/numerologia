namespace Numerologia.Core.Calculos;

public static class CalculoPiramide
{
    public static ResultadoPiramide Calcular(int[] valoresLetras)
    {
        var triangulo = new List<int[]> { valoresLetras };
        var linha = valoresLetras;

        while (linha.Length > 1)
        {
            var proxima = new int[linha.Length - 1];
            for (var i = 0; i < proxima.Length; i++)
            {
                var soma = linha[i] + linha[i + 1];
                proxima[i] = soma > 9 ? soma - 9 : soma;
            }
            triangulo.Add(proxima);
            linha = proxima;
        }

        var arcanos = new int[Math.Max(0, valoresLetras.Length - 1)];
        for (var i = 0; i < arcanos.Length; i++)
            arcanos[i] = valoresLetras[i] * 10 + valoresLetras[i + 1];

        var sequencias = DetectarSequencias(triangulo);

        return new ResultadoPiramide(triangulo.ToArray(), linha[0], arcanos, sequencias);
    }

    private static SequenciaNegativa[] DetectarSequencias(List<int[]> triangulo)
    {
        var resultado = new List<SequenciaNegativa>();

        for (var rowIdx = 0; rowIdx < triangulo.Count; rowIdx++)
        {
            var row = triangulo[rowIdx];
            var i = 0;
            while (i < row.Length - 2)
            {
                if (row[i] == row[i + 1] && row[i + 1] == row[i + 2])
                {
                    var digito = row[i];
                    var inicio = i;
                    var fim    = i + 2;
                    while (fim + 1 < row.Length && row[fim + 1] == digito)
                        fim++;

                    var significado = TabelaSequenciasNegativas.ObterSignificado(digito);
                    if (significado is not null)
                        resultado.Add(new SequenciaNegativa(rowIdx, inicio, fim - inicio + 1, digito, significado));

                    i = fim + 1;
                }
                else
                {
                    i++;
                }
            }
        }

        return resultado.ToArray();
    }
}

public record SequenciaNegativa(int Linha, int PosicaoInicio, int Comprimento, int Digito, string Significado);

public record ResultadoPiramide(int[][] Triangulo, int ArcanoMomento, int[] Arcanos, SequenciaNegativa[] SequenciasNegativas);
