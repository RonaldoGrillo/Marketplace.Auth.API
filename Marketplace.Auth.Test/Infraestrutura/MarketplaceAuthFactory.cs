using Marketplace.Auth.Aplicacao.Interfaces;
using Marketplace.Auth.Repositorio.Persistencia.Contexto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Auth.Test.Infraestrutura;

public class MarketplaceAuthFactory : WebApplicationFactory<Program>
{
    // Conexão mantida aberta para que o banco SQLite :memory: persista entre requests
    private readonly SqliteConnection _conexao = new("Data Source=:memory:");

    // Fake singleton exposto para que os testes possam inspecionar tokens capturados
    public readonly EmailServicoFake EmailFake = new();

    // Service provider interno com apenas o provider SQLite — evita o conflito
    // com o Npgsql que já está registrado nos serviços do app original
    private readonly IServiceProvider _efInternalSp = new ServiceCollection()
        .AddEntityFrameworkSqlite()
        .BuildServiceProvider();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _conexao.Open();

        builder.ConfigureServices(services =>
        {
            // Remove DbContextOptions<AuthDbContexto> E IDbContextOptionsConfiguration<AuthDbContexto>
            // (o EF Core 8+ armazena as options actions em IDbContextOptionsConfiguration,
            //  então apenas remover DbContextOptions não é suficiente)
            var toRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AuthDbContexto>) ||
                    d.ServiceType == typeof(IDbContextOptionsConfiguration<AuthDbContexto>))
                .ToList();

            foreach (var d in toRemove)
                services.Remove(d);

            services.AddDbContext<AuthDbContexto>(options =>
                options.UseSqlite(_conexao)
                       .UseInternalServiceProvider(_efInternalSp));

            // Substitui o EmailServico real pelo fake para evitar envios reais
            // e permitir que os testes capturem tokens de redefinição de senha
            var emailDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailServico));
            if (emailDescriptor is not null)
                services.Remove(emailDescriptor);
            services.AddSingleton<IEmailServico>(EmailFake);
        });

        builder.UseSetting("Jwt:Chave", "chave-super-secreta-para-testes-123456");
        builder.UseSetting("Jwt:Emissor", "MarketplaceAuthTest");
        builder.UseSetting("Jwt:Audiencia", "MarketplaceAuthTest");
        builder.UseSetting("Jwt:ExpiracaoMinutos", "60");
        builder.UseSetting("Jwt:RefreshTokenExpiracaoDias", "7");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _conexao.Dispose();
    }
}

