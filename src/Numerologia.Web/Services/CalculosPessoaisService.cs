using System.Net.Http.Json;

namespace Numerologia.Web.Services;

public class CalculosPessoaisService : ICalculosPessoaisService
{
    private readonly HttpClient _http;

    public CalculosPessoaisService(HttpClient http) => _http = http;

    public async Task<ResultadoPessoalDto> ObterAsync(int diaNascimento, int mesNascimento)
    {
        var result = await _http.GetFromJsonAsync<ResultadoPessoalDto>(
            $"/calculos/pessoal?dia={diaNascimento}&mes={mesNascimento}");
        return result!;
    }
}
