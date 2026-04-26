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

        var diasFavoraveis   = TabelaNumerosFavoraveis.GerarDiasFavoraveis(dataNascimento.Day, dataNascimento.Month);
        var harmonicos       = TabelaNumerosHarmonicos.Consultar(destino.DiaReduzido);
        var cores            = TabelaCoresFavoraveis.Consultar(mapa.NumeroExpressao);
        var harmonia         = TabelaHarmoniaConjugal.Consultar(destino.Missao);
        var dividas          = MergeDividas(mapa.DividasCarmicas, dataNascimento.Day, destino.NumeroDestino);

        return MapaNumerologico.Criar(
            consulenteId,
            nomeUtilizado,
            dataNascimento,
            mapa,
            destino,
            dividas,
            diasFavoraveis,
            harmonicos.SeHarmonizamCom,
            cores,
            harmonia);
    }

    public void Atualizar(MapaNumerologico mapaExistente, string novoNomeUtilizado, DateOnly? dataNascimento = null)
    {
        var dataEfetiva = dataNascimento ?? mapaExistente.DataNascimento;
        var mapa    = _calculoMapa.Calcular(novoNomeUtilizado);
        var destino = _calculoDestino.Calcular(dataEfetiva, mapa.NumeroExpressao);

        var diasFavoraveis = TabelaNumerosFavoraveis.GerarDiasFavoraveis(dataEfetiva.Day, dataEfetiva.Month);
        var harmonicos     = TabelaNumerosHarmonicos.Consultar(destino.DiaReduzido);
        var cores          = TabelaCoresFavoraveis.Consultar(mapa.NumeroExpressao);
        var harmonia       = TabelaHarmoniaConjugal.Consultar(destino.Missao);
        var dividas        = MergeDividas(mapa.DividasCarmicas, dataEfetiva.Day, destino.NumeroDestino);

        mapaExistente.Atualizar(
            novoNomeUtilizado,
            dataEfetiva,
            mapa,
            destino,
            dividas,
            diasFavoraveis,
            harmonicos.SeHarmonizamCom,
            cores,
            harmonia);
    }

    // Pág. 117: merge de todas as fontes de Dívidas Cármicas.
    private static int[] MergeDividas(
        IReadOnlyList<int> doNome, int diaNascimento, int numeroDestino)
    {
        var dividas = new HashSet<int>(doNome);

        // Dia de nascimento = 13, 14, 16 ou 19 → dívida direta
        if (diaNascimento is 13 or 14 or 16 or 19)
            dividas.Add(diaNascimento);

        // Destino = 4, 5, 7 ou 1 → indicador de dívida
        if (_reducaoParaDivida.TryGetValue(numeroDestino, out var dDestino))
            dividas.Add(dDestino);

        return [.. dividas.Order()];
    }

    private static readonly Dictionary<int, int> _reducaoParaDivida = new()
    {
        { 4, 13 }, { 5, 14 }, { 7, 16 }, { 1, 19 }
    };
}
