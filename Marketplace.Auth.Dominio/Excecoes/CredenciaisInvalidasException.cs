namespace Marketplace.Auth.Dominio.Excecoes;

public class CredenciaisInvalidasException : DominioException
{
    public CredenciaisInvalidasException()
        : base("E-mail ou senha inválidos.") { }
}
