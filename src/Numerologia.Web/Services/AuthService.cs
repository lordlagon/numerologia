using System.Net.Http.Json;

namespace Numerologia.Web.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http) => _http = http;

    public async Task<UsuarioInfo?> GetCurrentUserAsync()
    {
        try
        {
            var response = await _http.GetAsync("/auth/me");
            if (!response.IsSuccessStatusCode)
                return null;

            var data = await response.Content.ReadFromJsonAsync<MeResponse>();
            return data is null ? null : new UsuarioInfo(data.Nome, data.Email, data.NomeExibicao);
        }
        catch
        {
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        await _http.PostAsync("/auth/logout", null);
    }

    private record MeResponse(string Nome, string Email, string? NomeExibicao);
}
