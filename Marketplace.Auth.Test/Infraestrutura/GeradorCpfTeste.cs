namespace Marketplace.Auth.Test.Infraestrutura;

/// <summary>
/// Gera CPFs válidos (com dígitos verificadores corretos) para uso em testes.
/// </summary>
internal static class GeradorCpfTeste
{
    private static readonly Random _rng = Random.Shared;

    public static string Gerar()
    {
        Span<int> d = stackalloc int[11];
        do
        {
            for (var i = 0; i < 9; i++)
                d[i] = _rng.Next(0, 10);

            d[9] = CalcularDigito(d, 9);
            d[10] = CalcularDigito(d, 10);
        }
        while (TodosIguais(d));

        return string.Concat(d.ToArray().Select(x => x.ToString()));
    }

    private static int CalcularDigito(Span<int> d, int posicao)
    {
        var soma = 0;
        for (var i = 0; i < posicao; i++)
            soma += d[i] * (posicao + 1 - i);
        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    private static bool TodosIguais(Span<int> d)
    {
        for (var i = 1; i < d.Length; i++)
            if (d[i] != d[0]) return false;
        return true;
    }
}
