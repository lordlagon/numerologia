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

        return new ResultadoPiramide(triangulo.ToArray(), linha[0], arcanos);
    }
}

public record ResultadoPiramide(int[][] Triangulo, int ArcanoMomento, int[] Arcanos);
