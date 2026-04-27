namespace Numerologia.Web.Services;

public record UsuarioInfo(string Nome, string Email, string? NomeExibicao = null);

public interface IAuthService
{
    Task<UsuarioInfo?> GetCurrentUserAsync();
    Task LogoutAsync();
}
