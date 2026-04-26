namespace Numerologia.Core.Calculos;

public class CalculosPessoais
{
    public ResultadoPessoal Calcular(DateOnly dataNascimento, DateOnly dataAtual)
    {
        var diaBase = SomarDigitos(dataNascimento.Day);
        var mesBase = dataNascimento.Month;

        // Pág. 176: se o aniversário ainda não passou no ano corrente, usa o ano anterior
        var aniversarioPassou = dataNascimento.Month < dataAtual.Month ||
            (dataNascimento.Month == dataAtual.Month && dataNascimento.Day <= dataAtual.Day);
        var anoReferencia = aniversarioPassou ? dataAtual.Year : dataAtual.Year - 1;

        var anoPessoal = ReducaoNumerologica.Reduzir(diaBase + mesBase + SomarDigitos(anoReferencia));
        var mesPessoal = ReducaoNumerologica.Reduzir(anoPessoal + mesBase);
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
