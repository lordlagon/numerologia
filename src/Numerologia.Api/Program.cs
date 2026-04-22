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

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

// Diagnóstico temporário — remover após confirmar o deploy
app.MapGet("/debug/wwwroot", (IWebHostEnvironment env) =>
{
    var root = env.WebRootPath ?? "(null)";
    var frameworkPath = Path.Combine(root, "_framework");
    var exists = Directory.Exists(frameworkPath);
    var total = exists ? Directory.GetFiles(frameworkPath).Length : 0;
    var datFiles = exists
        ? Directory.GetFiles(frameworkPath, "*icu*").Select(Path.GetFileName)
        : Enumerable.Empty<string>();
    return Results.Ok(new { webRootPath = root, frameworkExists = exists, total, datFiles });
});

// Fallback para o roteamento client-side do Blazor
app.MapFallbackToFile("index.html");

app.Run();
