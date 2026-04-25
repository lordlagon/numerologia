using System.Net.Http.Json;

namespace Numerologia.Web.Services;

public class ConsulentesService : IConsulentesService
{
    private readonly HttpClient _http;

    public ConsulentesService(HttpClient http) => _http = http;

    public async Task<List<ConsulenteDto>> ListarAsync() =>
        await _http.GetFromJsonAsync<List<ConsulenteDto>>("/api/consulentes") ?? [];

    public async Task<ConsulenteDto?> ObterAsync(int id) =>
        await _http.GetFromJsonAsync<ConsulenteDto>($"/api/consulentes/{id}");

    public async Task<ConsulenteDto> CriarAsync(SalvarConsulenteDto dto)
    {
        var response = await _http.PostAsJsonAsync("/api/consulentes", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ConsulenteDto>())!;
    }

    public async Task<ConsulenteDto> AtualizarAsync(int id, SalvarConsulenteDto dto)
    {
        var response = await _http.PutAsJsonAsync($"/api/consulentes/{id}", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ConsulenteDto>())!;
    }

    public async Task RemoverAsync(int id)
    {
        var response = await _http.DeleteAsync($"/api/consulentes/{id}");
        response.EnsureSuccessStatusCode();
    }
}
