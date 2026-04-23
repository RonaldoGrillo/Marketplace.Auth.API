using Marketplace.Auth.Dominio.Excecoes;

namespace Marketplace.Auth.Dominio.Subentidades;

public record Senha
{
    public string Valor { get; }

    public Senha(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || valor.Length < 8)
            throw new DominioException("A senha deve ter pelo menos 8 caracteres.");

        Valor = valor;
    }

    public override string ToString() => Valor;
}
