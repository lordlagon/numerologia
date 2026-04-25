namespace Numerologia.Web.Services;

public interface IConsulentesService
{
    Task<List<ConsulenteDto>> ListarAsync();
    Task<ConsulenteDto?> ObterAsync(int id);
    Task<ConsulenteDto> CriarAsync(SalvarConsulenteDto dto);
    Task<ConsulenteDto> AtualizarAsync(int id, SalvarConsulenteDto dto);
    Task RemoverAsync(int id);
}

public record ConsulenteDto(int Id, string NomeCompleto, DateOnly DataNascimento,
    string? Email, string? Telefone, DateTime CriadoEm);

public record SalvarConsulenteDto(string NomeCompleto, string DataNascimento,
    string? Email, string? Telefone);
