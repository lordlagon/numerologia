using System.Net.Http.Json;

namespace Numerologia.Web.Services;

public class MapasService : IMapasService
{
    private readonly HttpClient _http;

    public MapasService(HttpClient http) => _http = http;

    public async Task<List<MapaResumoDto>> ListarAsync(int consulenteId)
    {
        var result = await _http.GetFromJsonAsync<List<MapaResumoDto>>(
            $"/api/consulentes/{consulenteId}/mapas");
        return result ?? [];
    }

    public async Task<MapaDetalheDto?> ObterAsync(int consulenteId, int mapaId)
        => await _http.GetFromJsonAsync<MapaDetalheDto>(
            $"/api/consulentes/{consulenteId}/mapas/{mapaId}");

    public async Task<MapaResumoDto> CriarAsync(int consulenteId, string nomeUtilizado)
    {
        var response = await _http.PostAsJsonAsync(
            $"/api/consulentes/{consulenteId}/mapas",
            new { NomeUtilizado = nomeUtilizado });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<MapaResumoDto>())!;
    }
}
