using System.Reflection;

namespace Marketplace.Auth.Utils.BancoDeDados;

public static class PostgreSqlTipoExtensoes
{
    private static readonly Dictionary<PostgreSqlTipo, PostgreSqlTipoInfoAttribute> _cache =
        Enum.GetValues<PostgreSqlTipo>()
            .ToDictionary(
                t => t,
                t => typeof(PostgreSqlTipo)
                         .GetField(t.ToString())!
                         .GetCustomAttribute<PostgreSqlTipoInfoAttribute>()
                     ?? throw new InvalidOperationException(
                         $"O valor '{t}' do enum PostgreSqlTipo nao possui o atributo [PostgreSqlTipoInfo]."));

    /// <summary>
    /// Retorna os metadados declarados via <see cref="PostgreSqlTipoInfoAttribute"/>
    /// (descricao, se aceita tamanho, limites, exemplo, etc.).
    /// </summary>
    public static PostgreSqlTipoInfoAttribute Info(this PostgreSqlTipo tipo) => _cache[tipo];

    /// <summary>
    /// Retorna a string SQL nativa do PostgreSQL correspondente ao tipo.
    /// Para tipos que aceitam tamanho informe <paramref name="tamanho"/> para gerar "tipo(n)".
    /// </summary>
    public static string ParaSql(this PostgreSqlTipo tipo, int? tamanho = null)
    {
        var info = _cache[tipo];

        if (tamanho.HasValue && !info.AceitaTamanho)
            throw new InvalidOperationException(
                $"O tipo '{tipo}' nao aceita parametro de tamanho no PostgreSQL.");

        if (!tamanho.HasValue && info.TamanhoObrigatorio)
            throw new InvalidOperationException(
                $"O tipo '{tipo}' requer um tamanho. Use ParaSql(tamanho: n).");

        if (tamanho.HasValue && info.TamanhoMinimo >= 0 && tamanho.Value < info.TamanhoMinimo)
            throw new ArgumentOutOfRangeException(nameof(tamanho),
                $"O tipo '{tipo}' exige tamanho minimo de {info.TamanhoMinimo}.");

        if (tamanho.HasValue && info.TamanhoMaximo >= 0 && tamanho.Value > info.TamanhoMaximo)
            throw new ArgumentOutOfRangeException(nameof(tamanho),
                $"O tipo '{tipo}' suporta tamanho maximo de {info.TamanhoMaximo}.");

        var nome = tipo switch
        {
            PostgreSqlTipo.Text => "text",
            PostgreSqlTipo.CharacterVarying => "character varying",
            PostgreSqlTipo.Character => "character",
            PostgreSqlTipo.Smallint => "smallint",
            PostgreSqlTipo.Integer => "integer",
            PostgreSqlTipo.Bigint => "bigint",
            PostgreSqlTipo.Bigserial => "bigserial",
            PostgreSqlTipo.Serial => "serial",
            PostgreSqlTipo.Smallserial => "smallserial",
            PostgreSqlTipo.Real => "real",
            PostgreSqlTipo.DoublePrecision => "double precision",
            PostgreSqlTipo.Numeric => "numeric",
            PostgreSqlTipo.Boolean => "boolean",
            PostgreSqlTipo.Date => "date",
            PostgreSqlTipo.Time => "time",
            PostgreSqlTipo.Timestamp => "timestamp",
            PostgreSqlTipo.TimestampWithTimeZone => "timestamp with time zone",
            PostgreSqlTipo.TimeWithTimeZone => "time with time zone",
            PostgreSqlTipo.Interval => "interval",
            PostgreSqlTipo.Uuid => "uuid",
            PostgreSqlTipo.Bytea => "bytea",
            PostgreSqlTipo.Json => "json",
            PostgreSqlTipo.Jsonb => "jsonb",
            _ => throw new ArgumentOutOfRangeException(nameof(tipo), tipo, "Tipo PostgreSQL nao mapeado.")
        };

        return tamanho.HasValue ? $"{nome}({tamanho})" : nome;
    }
}
