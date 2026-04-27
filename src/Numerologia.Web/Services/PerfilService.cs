using System.Net.Http.Json;

namespace Numerologia.Web.Services;

public class PerfilService : IPerfilService
{
    private readonly HttpClient _http;

    public PerfilService(HttpClient http) => _http = http;

    public async Task<PerfilDto> ObterAsync() =>
        (await _http.GetFromJsonAsync<PerfilDto>("/api/perfil"))!;

    public async Task<PerfilDto> AtualizarAsync(SalvarPerfilDto dto)
    {
        var response = await _http.PutAsJsonAsync("/api/perfil", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PerfilDto>())!;
    }
}
