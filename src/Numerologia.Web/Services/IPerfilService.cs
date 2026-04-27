namespace Numerologia.Web.Services;

public interface IPerfilService
{
    Task<PerfilDto> ObterAsync();
    Task<PerfilDto> AtualizarAsync(SalvarPerfilDto dto);
}

public record PerfilDto(string Email, string Nome, string? NomeExibicao);
public record SalvarPerfilDto(string? NomeExibicao);
