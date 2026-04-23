namespace Marketplace.Auth.Utils.Extensoes;

public static class DateTimeExtensoes
{
    public static bool Expirou(this DateTime dataExpiracao) =>
        dataExpiracao < DateTime.UtcNow;

    public static string FormatarBr(this DateTime data) =>
        data.ToString("dd/MM/yyyy HH:mm");
}
