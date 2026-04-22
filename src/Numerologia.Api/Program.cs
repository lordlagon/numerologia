var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Railway termina HTTPS no proxy — não redirecionar internamente
// app.UseHttpsRedirection();

// Serve o Blazor WASM (wwwroot/index.html na raiz)
app.UseDefaultFiles();
app.UseStaticFiles();

// Sanity check — remover quando a API real for implementada
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

// Fallback para o roteamento client-side do Blazor
app.MapFallbackToFile("index.html");

app.Run();
