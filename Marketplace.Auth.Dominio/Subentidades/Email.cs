using Marketplace.Auth.Dominio.Excecoes;
using Marketplace.Auth.Utils.Helpers;

namespace Marketplace.Auth.Dominio.Subentidades;

public record Email
{
    public string Valor { get; }

    public Email(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || !RegexHelper.ValidarEmail(valor))
            throw new DominioException("E-mail inválido.");

        Valor = valor.ToLowerInvariant();
    }

    public override string ToString() => Valor;
}
