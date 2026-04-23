namespace Marketplace.Auth.Utils.Extensoes;

public static class EnumerableExtensoes
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? colecao) =>
        colecao is null || !colecao.Any();
}
