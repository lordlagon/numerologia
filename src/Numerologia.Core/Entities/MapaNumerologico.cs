using Numerologia.Core.Calculos;

namespace Numerologia.Core.Entities;

public class MapaNumerologico
{
    public int      Id              { get; private set; }
    public int      ConsulenteId    { get; private set; }
    public string   NomeUtilizado   { get; private set; }
    public DateOnly DataNascimento  { get; private set; }
    public DateTime CriadoEm       { get; private set; }

    // Do nome
    public EntradaLetra[]       GradeLetras          { get; private set; }
    public int                  NumeroMotivacao      { get; private set; }
    public int                  NumeroImpressao      { get; private set; }
    public int                  NumeroExpressao      { get; private set; }
    public int[]                DividasCarmicas      { get; private set; }
    public Dictionary<int, int> FiguraA              { get; private set; }
    public int[]                LicoesCarmicas       { get; private set; }
    public int[]                TendenciasOcultas    { get; private set; }
    public int                  RespostaSubconsciente { get; private set; }

    // Da data de nascimento
    public int MesNascimentoReduzido { get; private set; }
    public int DiaNascimentoReduzido { get; private set; }
    public int AnoNascimentoReduzido { get; private set; }
    public int NumeroDestino         { get; private set; }
    public int Missao                { get; private set; }
    public int CicloVida1            { get; private set; }
    public int CicloVida2            { get; private set; }
    public int CicloVida3            { get; private set; }
    public int FimCiclo1Idade        { get; private set; }
    public int FimCiclo2Idade        { get; private set; }
    public int Desafio1              { get; private set; }
    public int Desafio2              { get; private set; }
    public int DesafioPrincipal      { get; private set; }
    public int MomentoDecisivo1      { get; private set; }
    public int MomentoDecisivo2      { get; private set; }
    public int MomentoDecisivo3      { get; private set; }
    public int MomentoDecisivo4      { get; private set; }
    public int[] DiasMesFavoraveis   { get; private set; }
    public int[] NumerosHarmonicos   { get; private set; }

    // Tabelas fixas
    public int      RelacaoIntervalores           { get; private set; }
    public int      HarmoniaVibraCom              { get; private set; }
    public int[]    HarmoniaAtrai                 { get; private set; }
    public int[]    HarmoniaEOpostoA              { get; private set; }
    public int[]    HarmoniaProfundamenteOpostoA  { get; private set; }
    public int[]    HarmoniaEPassivoEm            { get; private set; }
    public string[] CoresFavoraveis               { get; private set; }

    private MapaNumerologico()
    {
        // EF Core
        NomeUtilizado              = null!;
        GradeLetras                = [];
        DividasCarmicas            = [];
        FiguraA                    = [];
        LicoesCarmicas             = [];
        TendenciasOcultas          = [];
        DiasMesFavoraveis          = [];
        NumerosHarmonicos          = [];
        HarmoniaAtrai              = [];
        HarmoniaEOpostoA           = [];
        HarmoniaProfundamenteOpostoA = [];
        HarmoniaEPassivoEm         = [];
        CoresFavoraveis            = [];
    }

    public static MapaNumerologico Criar(
        int consulenteId,
        string nomeUtilizado,
        DateOnly dataNascimento,
        ResultadoMapa mapa,
        ResultadoDestino destino,
        int[] diasFavoraveis,
        int[] numerosHarmonicos,
        string[] coresFavoraveis,
        ResultadoHarmoniaConjugal harmonia)
    {
        return new MapaNumerologico
        {
            ConsulenteId   = consulenteId,
            NomeUtilizado  = nomeUtilizado,
            DataNascimento = dataNascimento,
            CriadoEm      = DateTime.UtcNow,

            GradeLetras           = [.. mapa.GradeLetras],
            NumeroMotivacao       = mapa.NumeroMotivacao,
            NumeroImpressao       = mapa.NumeroImpressao,
            NumeroExpressao       = mapa.NumeroExpressao,
            DividasCarmicas       = [.. mapa.DividasCarmicas],
            FiguraA               = new Dictionary<int, int>(mapa.FiguraA),
            LicoesCarmicas        = [.. mapa.LicoesCarmicas],
            TendenciasOcultas     = [.. mapa.TendenciasOcultas],
            RespostaSubconsciente = mapa.RespostaSubconsciente,

            MesNascimentoReduzido = destino.MesReduzido,
            DiaNascimentoReduzido = destino.DiaReduzido,
            AnoNascimentoReduzido = destino.AnoReduzido,
            NumeroDestino         = destino.NumeroDestino,
            Missao                = destino.Missao,
            CicloVida1            = destino.CicloVida1,
            CicloVida2            = destino.CicloVida2,
            CicloVida3            = destino.CicloVida3,
            FimCiclo1Idade        = destino.FimCiclo1Idade,
            FimCiclo2Idade        = destino.FimCiclo2Idade,
            Desafio1              = destino.Desafio1,
            Desafio2              = destino.Desafio2,
            DesafioPrincipal      = destino.DesafioPrincipal,
            MomentoDecisivo1      = destino.MomentoDecisivo1,
            MomentoDecisivo2      = destino.MomentoDecisivo2,
            MomentoDecisivo3      = destino.MomentoDecisivo3,
            MomentoDecisivo4      = destino.MomentoDecisivo4,

            DiasMesFavoraveis = diasFavoraveis,
            NumerosHarmonicos = numerosHarmonicos,

            RelacaoIntervalores          = mapa.NumeroImpressao - mapa.NumeroMotivacao,
            HarmoniaVibraCom             = harmonia.VibraCom,
            HarmoniaAtrai                = [.. harmonia.Atrai],
            HarmoniaEOpostoA             = [.. harmonia.EOpostoA],
            HarmoniaProfundamenteOpostoA = [.. harmonia.ProfundamenteOpostoA],
            HarmoniaEPassivoEm           = [.. harmonia.EPassivoEm],
            CoresFavoraveis              = coresFavoraveis,
        };
    }

    public void Atualizar(
        string nomeUtilizado,
        DateOnly dataNascimento,
        ResultadoMapa mapa,
        ResultadoDestino destino,
        int[] diasFavoraveis,
        int[] numerosHarmonicos,
        string[] coresFavoraveis,
        ResultadoHarmoniaConjugal harmonia)
    {
        NomeUtilizado  = nomeUtilizado;
        DataNascimento = dataNascimento;

        GradeLetras           = [.. mapa.GradeLetras];
        NumeroMotivacao       = mapa.NumeroMotivacao;
        NumeroImpressao       = mapa.NumeroImpressao;
        NumeroExpressao       = mapa.NumeroExpressao;
        DividasCarmicas       = [.. mapa.DividasCarmicas];
        FiguraA               = new Dictionary<int, int>(mapa.FiguraA);
        LicoesCarmicas        = [.. mapa.LicoesCarmicas];
        TendenciasOcultas     = [.. mapa.TendenciasOcultas];
        RespostaSubconsciente = mapa.RespostaSubconsciente;

        MesNascimentoReduzido = destino.MesReduzido;
        DiaNascimentoReduzido = destino.DiaReduzido;
        AnoNascimentoReduzido = destino.AnoReduzido;
        NumeroDestino         = destino.NumeroDestino;
        Missao                = destino.Missao;
        CicloVida1            = destino.CicloVida1;
        CicloVida2            = destino.CicloVida2;
        CicloVida3            = destino.CicloVida3;
        FimCiclo1Idade        = destino.FimCiclo1Idade;
        FimCiclo2Idade        = destino.FimCiclo2Idade;
        Desafio1              = destino.Desafio1;
        Desafio2              = destino.Desafio2;
        DesafioPrincipal      = destino.DesafioPrincipal;
        MomentoDecisivo1      = destino.MomentoDecisivo1;
        MomentoDecisivo2      = destino.MomentoDecisivo2;
        MomentoDecisivo3      = destino.MomentoDecisivo3;
        MomentoDecisivo4      = destino.MomentoDecisivo4;

        DiasMesFavoraveis = diasFavoraveis;
        NumerosHarmonicos = numerosHarmonicos;

        RelacaoIntervalores          = mapa.NumeroImpressao - mapa.NumeroMotivacao;
        HarmoniaVibraCom             = harmonia.VibraCom;
        HarmoniaAtrai                = [.. harmonia.Atrai];
        HarmoniaEOpostoA             = [.. harmonia.EOpostoA];
        HarmoniaProfundamenteOpostoA = [.. harmonia.ProfundamenteOpostoA];
        HarmoniaEPassivoEm           = [.. harmonia.EPassivoEm];
        CoresFavoraveis              = coresFavoraveis;
    }
}
