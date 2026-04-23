using System.Text.RegularExpressions;
using Marketplace.Auth.Utils.Constantes;

namespace Marketplace.Auth.Utils.Helpers;

public static class RegexHelper
{
    public static bool ValidarEmail(string valor) =>
        Regex.IsMatch(valor, RegexPadroes.Email, RegexOptions.IgnoreCase);

    public static bool ValidarSenha(string valor) =>
        Regex.IsMatch(valor, RegexPadroes.Senha);
}
