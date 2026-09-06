using AutoBrillo.Web;
using AutoBrillo.Web.Servicios;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// La dirección de la API se configura al publicar, sin acceso directo a la base de datos.
var apiUrl = builder.Configuration["ApiUrl"] ?? "https://localhost:7001/";
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiUrl) });
builder.Services.AddScoped<ServicioAutenticacion>();

await builder.Build().RunAsync();
