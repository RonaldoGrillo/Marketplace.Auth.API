namespace Marketplace.Auth.Dominio.Excecoes;

public class UsuarioNaoEncontradoException : DominioException
{
    public UsuarioNaoEncontradoException(Guid id)
        : base($"Usuário com ID '{id}' não foi encontrado.") { }

    public UsuarioNaoEncontradoException(string email)
        : base($"Usuário com e-mail '{email}' não foi encontrado.") { }
}
