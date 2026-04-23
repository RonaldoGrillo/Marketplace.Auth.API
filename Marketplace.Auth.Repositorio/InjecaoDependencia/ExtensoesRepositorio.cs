using Marketplace.Auth.Aplicacao.Interfaces;
using Marketplace.Auth.Dominio.Interfaces;
using Marketplace.Auth.Repositorio.Persistencia;
using Marketplace.Auth.Repositorio.Persistencia.Contexto;
using Marketplace.Auth.Repositorio.Persistencia.Repositorios;
using Marketplace.Auth.Repositorio.Servicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Auth.Repositorio.InjecaoDependencia;

public static class ExtensoesRepositorio
{
    public static IServiceCollection AddRepositorio(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContexto>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Padrao")));

        services.AddScoped<SincronizadorEsquema>();
        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        services.AddScoped<ITokenServico, TokenServico>();
        services.AddScoped<IEmailServico, EmailServico>();
        services.AddScoped<ISenhaCriptografiaServico, SenhaCriptografiaServico>();

        return services;
    }
}