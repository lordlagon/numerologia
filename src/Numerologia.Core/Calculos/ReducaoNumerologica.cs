namespace Numerologia.Core.Calculos;

public static class ReducaoNumerologica
{
    private static readonly HashSet<int> _mestres = [11, 22];

    public static int Reduzir(int numero)
    {
        while (numero > 9 && !_mestres.Contains(numero))
            numero = SomarDigitos(numero);

        return numero;
    }

    private static int SomarDigitos(int numero)
    {
        var soma = 0;
        while (numero > 0)
        {
            soma += numero % 10;
            numero /= 10;
        }
        return soma;
    }
}
