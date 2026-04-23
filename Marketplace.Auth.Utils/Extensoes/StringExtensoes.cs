namespace Marketplace.Auth.Utils.Extensoes;

public static class StringExtensoes
{
    public static bool IsNullOrEmpty(this string? valor) =>
        string.IsNullOrEmpty(valor);

    public static bool IsNullOrWhiteSpace(this string? valor) =>
        string.IsNullOrWhiteSpace(valor);

    public static string TrimOrEmpty(this string? valor) =>
        valor?.Trim() ?? string.Empty;

    public static string PrimeiraMaiuscula(this string valor) =>
        string.IsNullOrWhiteSpace(valor)
            ? valor
            : char.ToUpperInvariant(valor[0]) + valor[1..].ToLowerInvariant();
}
