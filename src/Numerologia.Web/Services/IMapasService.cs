namespace Numerologia.Web.Services;

public interface IMapasService
{
    Task<List<MapaResumoDto>> ListarAsync(int consulenteId);
    Task<MapaDetalheDto?> ObterAsync(int consulenteId, int mapaId);
    Task<MapaResumoDto> CriarAsync(int consulenteId, string nomeUtilizado);
    Task RemoverAsync(int consulenteId, int mapaId);
    Task<MapaResumoDto> AtualizarAsync(int consulenteId, int mapaId, string nomeUtilizado);
}

public interface IDashboardService
{
    Task<DashboardDto> ObterAsync();
}

public record DashboardDto(int TotalConsulentes, List<UltimoMapaDto> UltimosMapas);

public record UltimoMapaDto(
    int MapaId,
    int ConsulenteId,
    string NomeConsulente,
    string NomeUtilizado,
    DateTime CriadoEm);

public interface ICalculosPessoaisService
{
    Task<ResultadoPessoalDto> ObterAsync(int diaNascimento, int mesNascimento);
}

public record ResultadoPessoalDto(int AnoPessoal, int MesPessoal, int DiaPessoal);

public enum TipoLetraDto { Vogal, Consoante, Espaco }

public record EntradaLetraDto(string Letra, TipoLetraDto Tipo, int ValorCabalistico);

public record MapaResumoDto(
    int Id,
    string NomeUtilizado,
    DateOnly DataNascimento,
    int NumeroMotivacao,
    int NumeroImpressao,
    int NumeroExpressao,
    int NumeroDestino,
    DateTime CriadoEm);

public record MapaDetalheDto(
    int Id,
    string NomeUtilizado,
    DateOnly DataNascimento,
    DateTime CriadoEm,
    List<EntradaLetraDto> GradeLetras,
    int NumeroMotivacao,
    int NumeroImpressao,
    int NumeroExpressao,
    int[] DividasCarmicas,
    Dictionary<string, int> FiguraA,
    int[] LicoesCarmicas,
    int[] TendenciasOcultas,
    int RespostaSubconsciente,
    int MesNascimentoReduzido,
    int DiaNascimentoReduzido,
    int AnoNascimentoReduzido,
    int NumeroDestino,
    int Missao,
    int CicloVida1,
    int CicloVida2,
    int CicloVida3,
    int FimCiclo1Idade,
    int FimCiclo2Idade,
    int Desafio1,
    int Desafio2,
    int DesafioPrincipal,
    int MomentoDecisivo1,
    int MomentoDecisivo2,
    int MomentoDecisivo3,
    int MomentoDecisivo4,
    int[] DiasMesFavoraveis,
    int[] NumerosHarmonicos,
    int RelacaoIntervalores,
    int HarmoniaVibraCom,
    int[] HarmoniaAtrai,
    int[] HarmoniaEOpostoA,
    int[] HarmoniaProfundamenteOpostoA,
    int[] HarmoniaEPassivoEm,
    string[] CoresFavoraveis);
