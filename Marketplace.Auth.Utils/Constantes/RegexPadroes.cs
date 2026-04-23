namespace Marketplace.Auth.Utils.Constantes;

public static class RegexPadroes
{
    public const string Email = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    public const string Senha = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";
}
