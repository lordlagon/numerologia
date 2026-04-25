using System.Net.Http.Json;

namespace Numerologia.Web.Services;

public class DashboardService : IDashboardService
{
    private readonly HttpClient _http;

    public DashboardService(HttpClient http) => _http = http;

    public async Task<DashboardDto> ObterAsync()
    {
        var result = await _http.GetFromJsonAsync<DashboardDto>("/api/dashboard");
        return result ?? new DashboardDto(0, []);
    }
}
