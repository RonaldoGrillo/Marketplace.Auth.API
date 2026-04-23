namespace Marketplace.Auth.Utils.Extensoes;

public static class DecimalExtensoes
{
    public static string FormatarMoeda(this decimal valor, string cultura = "pt-BR") =>
        valor.ToString("C", new System.Globalization.CultureInfo(cultura));

    public static decimal Arredondar(this decimal valor, int casas = 2) =>
        Math.Round(valor, casas, MidpointRounding.AwayFromZero);
}
