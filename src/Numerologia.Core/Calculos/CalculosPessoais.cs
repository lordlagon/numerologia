namespace Numerologia.Core.Calculos;

public class CalculosPessoais
{
    public ResultadoPessoal Calcular(DateOnly dataNascimento, DateOnly dataAtual)
    {
        var diaBase = SomarDigitos(dataNascimento.Day);
        var mesBase = dataNascimento.Month;
        var anoAtual = SomarDigitos(dataAtual.Year);

        var anoPessoal = ReducaoNumerologica.Reduzir(diaBase + mesBase + anoAtual);
        var mesPessoal = ReducaoNumerologica.Reduzir(anoPessoal + dataAtual.Month);
        var diaPessoal = ReducaoNumerologica.Reduzir(mesPessoal + SomarDigitos(dataAtual.Day));

        return new ResultadoPessoal(anoPessoal, mesPessoal, diaPessoal);
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
