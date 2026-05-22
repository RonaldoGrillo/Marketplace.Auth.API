namespace Marketplace.Auth.Utils.Helpers;

/// <summary>
/// Valida CPF e CNPJ verificando os dígitos verificadores.
/// Aceita apenas dígitos (sem formatação).
/// </summary>
public static class DocumentoHelper
{
    public static bool ValidarCpf(string valor)
    {
        var cpf = ApenasDigitos(valor);
        if (cpf.Length != 11 || TodosIguais(cpf)) return false;

        return VerificaDigito(cpf, 9) && VerificaDigito(cpf, 10);
    }

    public static bool ValidarCnpj(string valor)
    {
        var cnpj = ApenasDigitos(valor);
        if (cnpj.Length != 14 || TodosIguais(cnpj)) return false;

        return VerificaDigitoCnpj(cnpj, 12) && VerificaDigitoCnpj(cnpj, 13);
    }

    /// <summary>Valida CPF ou CNPJ com base no comprimento do documento.</summary>
    public static bool Validar(string valor)
    {
        var digits = ApenasDigitos(valor);
        return digits.Length switch
        {
            11 => ValidarCpf(digits),
            14 => ValidarCnpj(digits),
            _ => false
        };
    }

    public static string ApenasDigitos(string valor) =>
        new(valor.Where(char.IsDigit).ToArray());

    private static bool TodosIguais(string s) => s.Distinct().Count() == 1;

    private static bool VerificaDigito(string cpf, int posicao)
    {
        var soma = 0;
        for (var i = 0; i < posicao; i++)
            soma += (cpf[i] - '0') * (posicao + 1 - i);

        var resto = soma % 11;
        var digito = resto < 2 ? 0 : 11 - resto;
        return (cpf[posicao] - '0') == digito;
    }

    private static bool VerificaDigitoCnpj(string cnpj, int posicao)
    {
        int[] pesos = posicao == 12
            ? [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]
            : [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var soma = 0;
        for (var i = 0; i < posicao; i++)
            soma += (cnpj[i] - '0') * pesos[i];

        var resto = soma % 11;
        var digito = resto < 2 ? 0 : 11 - resto;
        return (cnpj[posicao] - '0') == digito;
    }
}
