var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Railway termina HTTPS no proxy — não redirecionar internamente
// app.UseHttpsRedirection();

// Blazor WASM usa extensões que o ASP.NET Core não reconhece por padrão (.dat, .blat)
// Sem esse mapeamento o StaticFileMiddleware retorna 404 para os arquivos _framework/
var contentTypeProvider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".dat"]  = "application/octet-stream";
contentTypeProvider.Mappings[".blat"] = "application/octet-stream";
contentTypeProvider.Mappings[".wasm"] = "application/wasm";

app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypeProvider
});

// Sanity check — remover quando a API real for implementada
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

// Fallback para o roteamento client-side do Blazor
app.MapFallbackToFile("index.html");

app.Run();
