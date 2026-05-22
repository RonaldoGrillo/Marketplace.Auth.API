using FluentValidation;
using Marketplace.Auth.Aplicacao.Servicos;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Auth.Aplicacao.InjecaoDependencia;

public static class ExtensoesAplicacao
{
    public static IServiceCollection AddAplicacao(this IServiceCollection services)
    {
        var assembly = typeof(ExtensoesAplicacao).Assembly;

        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<UsuarioServico>();
        services.AddScoped<AutenticacaoServico>();

        return services;
    }
}
