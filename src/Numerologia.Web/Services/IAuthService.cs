namespace Numerologia.Web.Services;

public record UsuarioInfo(string Nome, string Email);

public interface IAuthService
{
    Task<UsuarioInfo?> GetCurrentUserAsync();
    Task LogoutAsync();
}
