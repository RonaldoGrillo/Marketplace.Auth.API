using FluentValidation;
using MapsterMapper;
using Marketplace.Auth.Aplicacao.Comportamentos;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Auth.Aplicacao.InjecaoDependencia;

public static class ExtensoesAplicacao
{
    public static IServiceCollection AddAplicacao(this IServiceCollection services)
    {
        var assembly = typeof(ExtensoesAplicacao).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(MediatR.IPipelineBehavior<,>), typeof(ComportamentoValidacao<,>));
        });
        services.AddValidatorsFromAssembly(assembly);

        var config = Mapster.TypeAdapterConfig.GlobalSettings;
        config.Scan(assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
