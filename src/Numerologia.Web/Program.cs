using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Numerologia.Web;
using Numerologia.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IConsulentesService, ConsulentesService>();
builder.Services.AddScoped<IMapasService, MapasService>();
builder.Services.AddScoped<ICalculosPessoaisService, CalculosPessoaisService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IAssinaturasService, AssinaturasService>();
builder.Services.AddSingleton<PerfilState>();

await builder.Build().RunAsync();
