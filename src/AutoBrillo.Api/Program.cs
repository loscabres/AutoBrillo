using System.Text;
using AutoBrillo.Repository.Datos;
using AutoBrillo.Repository.Repositorios;
using AutoBrillo.Repository.Seguridad;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Vercel Services indica el puerto mediante PORT; localmente ASP.NET usa el habitual.
var puerto = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(puerto))
    builder.WebHost.UseUrls($"http://0.0.0.0:{puerto}");

// Las credenciales se leen de variables de entorno, no del código fuente.
var cadenaConexion = builder.Configuration["CONNECTION_STRING"]
    ?? throw new InvalidOperationException("Falta la variable de entorno CONNECTION_STRING.");
var jwtSecret = builder.Configuration["JWT_SECRET"]
    ?? throw new InvalidOperationException("Falta la variable de entorno JWT_SECRET.");
var jwtIssuer = builder.Configuration["JWT_ISSUER"] ?? "AutoBrillo.Api";
var jwtAudience = builder.Configuration["JWT_AUDIENCE"] ?? "AutoBrillo.Web";

builder.Services.AddDbContext<AutoBrilloDbContext>(options => options.UseNpgsql(cadenaConexion));
builder.Services.AddScoped<IUserRepository, RepositorioUsuarios>();
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddControllers();

// En producción solo se permite la URL publicada de la aplicación web.
var origenWeb = builder.Configuration["WEB_ORIGIN"];
builder.Services.AddCors(options => options.AddPolicy("Web", policy =>
{
    policy.AllowAnyHeader().AllowAnyMethod();
    if (string.IsNullOrWhiteSpace(origenWeb))
        policy.AllowAnyOrigin(); // Útil únicamente durante desarrollo local.
    else
        policy.WithOrigins(origenWeb);
}));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors("Web");
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new { estado = "correcto" }));
app.MapControllers();
app.Run();
