namespace Numerologia.Core.Calculos;

public class CalculoDestino
{
    public ResultadoDestino Calcular(DateOnly dataNascimento, int numeroExpressao)
    {
        var dia = dataNascimento.Day;
        var mes = dataNascimento.Month;
        var ano = dataNascimento.Year;

        var destino = CalcularDestino(dia, mes, ano);
        var missao  = ReducaoNumerologica.Reduzir(destino + numeroExpressao);

        var mesReduzido = ReducaoNumerologica.Reduzir(mes);
        var diaReduzido = ReducaoNumerologica.Reduzir(SomarDigitos(dia));
        var anoReduzido = ReducaoNumerologica.Reduzir(SomarDigitos(ano));

        var fimCiclo1 = 36 - destino;
        var desafio1  = Math.Abs(mesReduzido - diaReduzido);
        var desafio2  = Math.Abs(anoReduzido - diaReduzido);

        var md1 = ReducaoNumerologica.Reduzir(diaReduzido + mesReduzido);
        var md2 = ReducaoNumerologica.Reduzir(diaReduzido + anoReduzido);
        var md3 = ReducaoNumerologica.Reduzir(md1 + md2);
        var md4 = ReducaoNumerologica.Reduzir(mesReduzido + anoReduzido);

        return new ResultadoDestino(
            NumeroDestino:    destino,
            Missao:           missao,
            MesReduzido:      mesReduzido,
            DiaReduzido:      diaReduzido,
            AnoReduzido:      anoReduzido,
            CicloVida1:       mesReduzido,
            CicloVida2:       diaReduzido,
            CicloVida3:       anoReduzido,
            FimCiclo1Idade:   fimCiclo1,
            FimCiclo2Idade:   fimCiclo1 + 27,
            Desafio1:         desafio1,
            Desafio2:         desafio2,
            DesafioPrincipal: Math.Abs(desafio2 - desafio1),
            MomentoDecisivo1: md1,
            MomentoDecisivo2: md2,
            MomentoDecisivo3: md3,
            MomentoDecisivo4: md4
        );
    }

    private static int CalcularDestino(int dia, int mes, int ano)
    {
        var soma = SomarDigitos(dia) + SomarDigitos(mes) + SomarDigitos(ano);
        return ReducaoNumerologica.Reduzir(soma);
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
