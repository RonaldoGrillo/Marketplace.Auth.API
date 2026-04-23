using Marketplace.Auth.Repositorio.Persistencia.Contexto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;

namespace Marketplace.Auth.Repositorio.Persistencia;

/// <summary>
/// Sincroniza o esquema do banco de dados com o modelo EF Core a cada inicialização,
/// sem depender de arquivos de migration.
///
/// Comportamento:
///   - Primeira execução ou Database:RecriarAoIniciar=true → recria o banco do zero.
///   - Execuções seguintes → adiciona colunas ausentes automaticamente.
///   - Alterações de tipo ou remoção de colunas → ative RecriarAoIniciar=true uma vez.
/// </summary>
public class SincronizadorEsquema(AuthDbContexto contexto, IConfiguration configuration, ILogger<SincronizadorEsquema> logger)
{
    public async Task SincronizarAsync(CancellationToken ct = default)
    {
        var recriar = bool.TryParse(configuration["Database:RecriarAoIniciar"], out var v) && v;

        if (recriar)
        {
            logger.LogWarning("Database:RecriarAoIniciar está ativo — recriando banco de dados...");
            await contexto.Database.EnsureDeletedAsync(ct);
            await contexto.Database.EnsureCreatedAsync(ct);
            logger.LogInformation("Banco de dados recriado com sucesso.");
            return;
        }

        var criado = await contexto.Database.EnsureCreatedAsync(ct);

        if (criado)
        {
            logger.LogInformation("Esquema do banco de dados criado com sucesso.");
            return;
        }

        await SincronizarColunasAsync(ct);
    }

    private async Task SincronizarColunasAsync(CancellationToken ct)
    {
        logger.LogInformation("Verificando atualizações no esquema do banco de dados...");

        var conn = contexto.Database.GetDbConnection();

        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        try
        {
            var colunasDb = await ObterColunasAtualAsync(conn, ct);
            var alteracoes = 0;

            foreach (var entidade in contexto.Model.GetEntityTypes())
            {
                var tabela = entidade.GetTableName();
                if (tabela is null) continue;

                if (!colunasDb.TryGetValue(tabela, out var colunasTabela))
                {
                    logger.LogWarning(
                        "Tabela '{Tabela}' não existe no banco. " +
                        "Ative 'Database:RecriarAoIniciar: true' para recriar o esquema.", tabela);
                    continue;
                }

                foreach (var prop in entidade.GetProperties())
                {
                    var nomeCol = ObterNomeColuna(entidade, prop);
                    if (nomeCol is null || colunasTabela.Contains(nomeCol)) continue;

                    await AdicionarColunaAsync(conn, tabela, entidade, prop, nomeCol, ct);
                    alteracoes++;
                }
            }

            if (alteracoes == 0)
                logger.LogInformation("Esquema do banco de dados está atualizado.");
            else
                logger.LogInformation("{N} coluna(s) adicionada(s) ao esquema.", alteracoes);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
                await conn.CloseAsync();
        }
    }

    private async Task AdicionarColunaAsync(DbConnection conn, string tabela, IEntityType entidade, IProperty prop, string nomeCol, CancellationToken ct)
    {
        var tipo = ObterTipoColuna(entidade, prop);
        var notNull = prop.IsNullable ? "" : $" NOT NULL DEFAULT {ObterValorPadrao(prop)}";

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"""ALTER TABLE "{tabela}" ADD COLUMN IF NOT EXISTS "{nomeCol}" {tipo}{notNull};""";
        await cmd.ExecuteNonQueryAsync(ct);

        logger.LogInformation("  + '{Tabela}'.'{Col}' ({Tipo}) adicionada.", tabela, nomeCol, tipo);
    }

    private static async Task<Dictionary<string, HashSet<string>>> ObterColunasAtualAsync(DbConnection conn, CancellationToken ct)
    {
        var resultado = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT table_name, column_name
            FROM information_schema.columns
            WHERE table_schema = 'public'
            ORDER BY table_name, ordinal_position
            """;

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var tabela = reader.GetString(0);
            var coluna = reader.GetString(1);

            if (!resultado.TryGetValue(tabela, out var cols))
                resultado[tabela] = cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            cols.Add(coluna);
        }

        return resultado;
    }

    private static string? ObterNomeColuna(IEntityType entidade, IProperty prop)
    {
        var storeObj = StoreObjectIdentifier.Table(entidade.GetTableName()!, entidade.GetSchema());
        return prop.GetColumnName(storeObj);
    }

    private static string ObterTipoColuna(IEntityType entidade, IProperty prop)
    {
        var storeObj = StoreObjectIdentifier.Table(entidade.GetTableName()!, entidade.GetSchema());

        // Tipo explicitamente configurado via HasColumnType() tem prioridade
        var tipoExplicito = prop.GetColumnType(storeObj);
        if (!string.IsNullOrEmpty(tipoExplicito)) return tipoExplicito;

        // Fallback baseado no tipo CLR + MaxLength
        var maxLen = prop.GetMaxLength();
        var clrType = Nullable.GetUnderlyingType(prop.ClrType) ?? prop.ClrType;

        return clrType switch
        {
            _ when clrType == typeof(Guid) => "uuid",
            _ when clrType == typeof(string) => maxLen.HasValue ? $"character varying({maxLen})" : "text",
            _ when clrType == typeof(int) => "integer",
            _ when clrType == typeof(long) => "bigint",
            _ when clrType == typeof(bool) => "boolean",
            _ when clrType == typeof(DateTime) => "timestamp with time zone",
            _ when clrType == typeof(decimal) => "numeric",
            _ when clrType.IsEnum => "character varying(50)",
            _ => "text"
        };
    }

    private static string ObterValorPadrao(IProperty prop)
    {
        var clrType = Nullable.GetUnderlyingType(prop.ClrType) ?? prop.ClrType;

        return clrType switch
        {
            _ when clrType == typeof(Guid) => "gen_random_uuid()",
            _ when clrType == typeof(string) => "''",
            _ when clrType == typeof(bool) => "false",
            _ when clrType == typeof(DateTime) => "NOW()",
            _ when clrType.IsEnum => "''",
            _ => "0"
        };
    }
}
