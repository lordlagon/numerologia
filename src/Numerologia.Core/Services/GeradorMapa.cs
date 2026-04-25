using Numerologia.Core.Calculos;
using Numerologia.Core.Entities;

namespace Numerologia.Core.Services;

public class GeradorMapa
{
    private readonly CalculoMapa    _calculoMapa    = new();
    private readonly CalculoDestino _calculoDestino = new();

    public MapaNumerologico Gerar(int consulenteId, string nomeUtilizado, DateOnly dataNascimento)
    {
        var mapa    = _calculoMapa.Calcular(nomeUtilizado);
        var destino = _calculoDestino.Calcular(dataNascimento, mapa.NumeroExpressao);

        var diasFavoraveis   = TabelaNumerosFavoraveis.Consultar(dataNascimento.Day, dataNascimento.Month);
        var harmonicos       = TabelaNumerosHarmonicos.Consultar(destino.DiaReduzido);
        var cores            = TabelaCoresFavoraveis.Consultar(mapa.NumeroExpressao);
        var harmonia         = TabelaHarmoniaConjugal.Consultar(mapa.NumeroExpressao);

        return MapaNumerologico.Criar(
            consulenteId,
            nomeUtilizado,
            dataNascimento,
            mapa,
            destino,
            diasFavoraveis,
            harmonicos.SeHarmonizamCom,
            cores,
            harmonia);
    }
}
