using Marketplace.Auth.Aplicacao.InjecaoDependencia;
using Marketplace.Auth.API.Middlewares;
using Marketplace.Auth.Repositorio.InjecaoDependencia;
using Marketplace.Auth.Repositorio.Persistencia;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddAplicacao();
builder.Services.AddRepositorio(builder.Configuration);

var jwtConfig = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtConfig["Chave"]!)),
            ValidateIssuer = true,
            ValidIssuer = jwtConfig["Emissor"],
            ValidateAudience = true,
            ValidAudience = jwtConfig["Audiencia"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Sincroniza o esquema do banco de dados com o modelo EF Core a cada inicialização
await using (var escopo = app.Services.CreateAsyncScope())
{
    var sincronizador = escopo.ServiceProvider.GetRequiredService<SincronizadorEsquema>();
    await sincronizador.SincronizarAsync();
}

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseMiddleware<TratamentoExcecoesMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
