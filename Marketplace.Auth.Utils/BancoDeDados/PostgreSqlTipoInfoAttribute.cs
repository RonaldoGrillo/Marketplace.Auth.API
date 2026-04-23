namespace Marketplace.Auth.Utils.BancoDeDados;

/// <summary>
/// Descreve as características de um tipo de dado PostgreSQL diretamente na declaração do enum.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class PostgreSqlTipoInfoAttribute(string descricao) : Attribute
{
    /// <summary>Descrição legível do tipo e seu uso típico.</summary>
    public string Descricao { get; } = descricao;

    /// <summary>Indica se o tipo aceita um parâmetro de tamanho/precisão entre parênteses.</summary>
    public bool AceitaTamanho { get; init; }

    /// <summary>Indica se o tamanho é obrigatório (ex.: character varying exige n).</summary>
    public bool TamanhoObrigatorio { get; init; }

    /// <summary>Tamanho ou precisão mínima aceita. -1 = sem restrição definida pelo PostgreSQL.</summary>
    public int TamanhoMinimo { get; init; } = -1;

    /// <summary>Tamanho ou precisão máxima aceita. -1 = sem restrição definida pelo PostgreSQL.</summary>
    public int TamanhoMaximo { get; init; } = -1;

    /// <summary>Exemplo de uso na DDL (ex.: "character varying(100)", "numeric(10,2)").</summary>
    public string Exemplo { get; init; } = string.Empty;
}
