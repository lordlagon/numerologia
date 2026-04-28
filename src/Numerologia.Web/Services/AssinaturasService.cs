using System.Net.Http.Json;

namespace Numerologia.Web.Services;

public class AssinaturasService : IAssinaturasService
{
    private readonly HttpClient _http;

    public AssinaturasService(HttpClient http) => _http = http;

    public async Task<AssinaturaPreviewDto?> PreviewAsync(int consulenteId, int mapaId, string texto)
    {
        var response = await _http.PostAsJsonAsync(
            $"/api/consulentes/{consulenteId}/mapas/{mapaId}/assinaturas/preview",
            new { Texto = texto });
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AssinaturaPreviewDto>();
    }

    public async Task<List<AssinaturaDto>> ListarAsync(int consulenteId, int mapaId)
    {
        var result = await _http.GetFromJsonAsync<List<AssinaturaDto>>(
            $"/api/consulentes/{consulenteId}/mapas/{mapaId}/assinaturas");
        return result ?? [];
    }

    public async Task<AssinaturaDto> SalvarAsync(int consulenteId, int mapaId, string texto)
    {
        var response = await _http.PostAsJsonAsync(
            $"/api/consulentes/{consulenteId}/mapas/{mapaId}/assinaturas",
            new { Texto = texto });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AssinaturaDto>())!;
    }

    public async Task EscolherAsync(int consulenteId, int mapaId, int assinaturaId)
    {
        var response = await _http.PutAsync(
            $"/api/consulentes/{consulenteId}/mapas/{mapaId}/assinaturas/{assinaturaId}/escolher", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task ExcluirAsync(int consulenteId, int mapaId, int assinaturaId)
    {
        var response = await _http.DeleteAsync(
            $"/api/consulentes/{consulenteId}/mapas/{mapaId}/assinaturas/{assinaturaId}");
        response.EnsureSuccessStatusCode();
    }
}
