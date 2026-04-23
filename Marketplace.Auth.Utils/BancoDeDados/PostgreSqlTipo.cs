namespace Marketplace.Auth.Utils.BancoDeDados;

/// <summary>
/// Tipos de dados nativos do PostgreSQL.
/// Cada valor carrega metadados via <see cref="PostgreSqlTipoInfoAttribute"/>:
/// descrição, se aceita tamanho, limites e exemplo de DDL.
/// Use <see cref="PostgreSqlTipoExtensoes.ParaSql"/> para obter a string SQL
/// e <see cref="PostgreSqlTipoExtensoes.Info"/> para acessar os metadados.
/// </summary>
public enum PostgreSqlTipo
{
    // ── Texto ────────────────────────────────────────────────────────────────

    [PostgreSqlTipoInfo("Texto de comprimento variável sem limite definido. " +
                        "Use quando o tamanho máximo não é conhecido ou é muito variável.",
        AceitaTamanho = false,
        Exemplo = "text")]
    Text,

    [PostgreSqlTipoInfo("Texto de comprimento variável com limite máximo obrigatório (varchar). " +
                        "Preferível ao 'text' quando há um teto natural (e-mail, nome, etc.).",
        AceitaTamanho = true, TamanhoObrigatorio = true,
        TamanhoMinimo = 1, TamanhoMaximo = 10_485_760,
        Exemplo = "character varying(255)")]
    CharacterVarying,

    [PostgreSqlTipoInfo("Texto de comprimento fixo com preenchimento de espaços (char/bpchar). " +
                        "Use apenas quando todos os valores têm exatamente o mesmo comprimento (ex.: código de país 'BR').",
        AceitaTamanho = true, TamanhoObrigatorio = true,
        TamanhoMinimo = 1, TamanhoMaximo = 10_485_760,
        Exemplo = "character(2)")]
    Character,

    // ── Inteiros ─────────────────────────────────────────────────────────────

    [PostgreSqlTipoInfo("Inteiro de 2 bytes. Intervalo: -32.768 a 32.767. " +
                        "Use para enums, flags ou contadores pequenos onde economizar espaço importa.",
        AceitaTamanho = false,
        Exemplo = "smallint")]
    Smallint,

    [PostgreSqlTipoInfo("Inteiro de 4 bytes. Intervalo: -2.147.483.648 a 2.147.483.647. " +
                        "Tipo inteiro padrão para a maioria das colunas.",
        AceitaTamanho = false,
        Exemplo = "integer")]
    Integer,

    [PostgreSqlTipoInfo("Inteiro de 8 bytes. Intervalo: -9.223.372.036.854.775.808 a 9.223.372.036.854.775.807. " +
                        "Use para IDs de alto volume ou valores monetários em centavos.",
        AceitaTamanho = false,
        Exemplo = "bigint")]
    Bigint,

    [PostgreSqlTipoInfo("Inteiro de 8 bytes auto-incrementado (equivale a bigint + sequence). " +
                        "Recomendado para chaves primárias de tabelas de alto volume.",
        AceitaTamanho = false,
        Exemplo = "bigserial")]
    Bigserial,

    [PostgreSqlTipoInfo("Inteiro de 4 bytes auto-incrementado (equivale a integer + sequence). " +
                        "Uso comum para chaves primárias; prefira 'bigserial' em tabelas de crescimento indeterminado.",
        AceitaTamanho = false,
        Exemplo = "serial")]
    Serial,

    [PostgreSqlTipoInfo("Inteiro de 2 bytes auto-incrementado (equivale a smallint + sequence). " +
                        "Use apenas quando o número total de linhas for sempre menor que 32.767.",
        AceitaTamanho = false,
        Exemplo = "smallserial")]
    Smallserial,

    // ── Ponto flutuante ───────────────────────────────────────────────────────

    [PostgreSqlTipoInfo("Ponto flutuante de precisão simples (4 bytes, ~6 dígitos significativos). " +
                        "Evite para valores financeiros devido a erros de arredondamento.",
        AceitaTamanho = false,
        Exemplo = "real")]
    Real,

    [PostgreSqlTipoInfo("Ponto flutuante de precisão dupla (8 bytes, ~15 dígitos significativos). " +
                        "Evite para valores financeiros; prefira 'numeric' nesses casos.",
        AceitaTamanho = false,
        Exemplo = "double precision")]
    DoublePrecision,

    [PostgreSqlTipoInfo("Numérico de precisão exata e escala configuráveis. " +
                        "Ideal para valores monetários ou qualquer cálculo que exija precisão decimal garantida. " +
                        "Sem parâmetros aceita valores de qualquer precisão.",
        AceitaTamanho = true, TamanhoObrigatorio = false,
        TamanhoMinimo = 1, TamanhoMaximo = 1000,
        Exemplo = "numeric(15,2)")]
    Numeric,

    // ── Lógico ────────────────────────────────────────────────────────────────

    [PostgreSqlTipoInfo("Valor booleano (true/false/null). " +
                        "Aceita entradas como 'true', 'yes', '1', 'on' e seus opostos.",
        AceitaTamanho = false,
        Exemplo = "boolean")]
    Boolean,

    // ── Data e hora ───────────────────────────────────────────────────────────

    [PostgreSqlTipoInfo("Data do calendário (ano, mês, dia) sem hora. Intervalo: 4713 AC a 5874897 DC.",
        AceitaTamanho = false,
        Exemplo = "date")]
    Date,

    [PostgreSqlTipoInfo("Hora do dia sem data e sem fuso horário. " +
                        "Aceita precisão de frações de segundo (0–6). Padrão: 6.",
        AceitaTamanho = true, TamanhoObrigatorio = false,
        TamanhoMinimo = 0, TamanhoMaximo = 6,
        Exemplo = "time(0)")]
    Time,

    [PostgreSqlTipoInfo("Data e hora sem informação de fuso horário. " +
                        "Aceita precisão de frações de segundo (0–6). " +
                        "Prefira 'timestamp with time zone' em sistemas distribuídos ou multi-região.",
        AceitaTamanho = true, TamanhoObrigatorio = false,
        TamanhoMinimo = 0, TamanhoMaximo = 6,
        Exemplo = "timestamp(0)")]
    Timestamp,

    [PostgreSqlTipoInfo("Data e hora com conversão automática para UTC (timestamptz). " +
                        "Recomendado para qualquer aplicação que possa operar em mais de um fuso horário.",
        AceitaTamanho = true, TamanhoObrigatorio = false,
        TamanhoMinimo = 0, TamanhoMaximo = 6,
        Exemplo = "timestamp with time zone")]
    TimestampWithTimeZone,

    [PostgreSqlTipoInfo("Hora do dia com fuso horário. Uso raro; prefira 'timestamptz' na maioria dos casos.",
        AceitaTamanho = true, TamanhoObrigatorio = false,
        TamanhoMinimo = 0, TamanhoMaximo = 6,
        Exemplo = "time with time zone")]
    TimeWithTimeZone,

    [PostgreSqlTipoInfo("Intervalo de tempo (duração). " +
                        "Útil para expiração de tokens, diferença entre datas, SLAs, etc.",
        AceitaTamanho = true, TamanhoObrigatorio = false,
        TamanhoMinimo = 0, TamanhoMaximo = 6,
        Exemplo = "interval")]
    Interval,

    // ── Identificadores ───────────────────────────────────────────────────────

    [PostgreSqlTipoInfo("Identificador único universal de 128 bits (16 bytes). " +
                        "Formato padrão: 'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx' (36 chars com hífens). " +
                        "Recomendado para chaves primárias distribuídas.",
        AceitaTamanho = false,
        Exemplo = "uuid")]
    Uuid,

    // ── Binário ───────────────────────────────────────────────────────────────

    [PostgreSqlTipoInfo("Sequência de bytes de comprimento variável. " +
                        "Use para armazenar arquivos, imagens ou qualquer dado binário diretamente no banco.",
        AceitaTamanho = false,
        Exemplo = "bytea")]
    Bytea,

    // ── JSON ──────────────────────────────────────────────────────────────────

    [PostgreSqlTipoInfo("Dados JSON armazenados como texto sem processamento interno. " +
                        "Preserva espaços e ordem das chaves. Use 'jsonb' para consultas e índices.",
        AceitaTamanho = false,
        Exemplo = "json")]
    Json,

    [PostgreSqlTipoInfo("Dados JSON armazenados em formato binário decomposto. " +
                        "Suporta índices GIN e operadores de consulta. Preferível ao 'json' na maioria dos casos.",
        AceitaTamanho = false,
        Exemplo = "jsonb")]
    Jsonb
}
