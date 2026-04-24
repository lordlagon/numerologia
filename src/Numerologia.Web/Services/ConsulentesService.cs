using System.Net.Http.Json;

namespace Numerologia.Web.Services;

public class ConsulentesService : IConsulentesService
{
    private readonly HttpClient _http;

    public ConsulentesService(HttpClient http) => _http = http;

    public async Task<List<ConsulenteDto>> ListarAsync() =>
        await _http.GetFromJsonAsync<List<ConsulenteDto>>("/consulentes") ?? [];

    public async Task<ConsulenteDto?> ObterAsync(int id) =>
        await _http.GetFromJsonAsync<ConsulenteDto>($"/consulentes/{id}");

    public async Task<ConsulenteDto> CriarAsync(SalvarConsulenteDto dto)
    {
        var response = await _http.PostAsJsonAsync("/consulentes", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ConsulenteDto>())!;
    }

    public async Task<ConsulenteDto> AtualizarAsync(int id, SalvarConsulenteDto dto)
    {
        var response = await _http.PutAsJsonAsync($"/consulentes/{id}", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ConsulenteDto>())!;
    }

    public async Task RemoverAsync(int id)
    {
        var response = await _http.DeleteAsync($"/consulentes/{id}");
        response.EnsureSuccessStatusCode();
    }
}
