using System.Security.Cryptography;
using System.Text;

namespace Marketplace.Auth.Utils.Helpers;

public static class CriptografiaHelper
{
    public static string GerarSha256(string valor)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(valor));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static string GerarTokenAleatorio(int tamanhoBytes = 64) =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(tamanhoBytes));
}
