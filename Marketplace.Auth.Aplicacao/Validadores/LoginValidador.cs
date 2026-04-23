using FluentValidation;
using Marketplace.Auth.Aplicacao.UseCases.Autenticacao.Login;

namespace Marketplace.Auth.Aplicacao.Validadores;

public class LoginValidador : AbstractValidator<LoginCommand>
{
    public LoginValidador()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("Informe um e-mail válido.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.");
    }
}
