using FluentValidation;
using Marketplace.Auth.Aplicacao.UseCases.Usuarios;

namespace Marketplace.Auth.Aplicacao.Validadores;

public class AtualizarUsuarioValidador : AbstractValidator<AtualizarUsuarioCommand>
{
    public AtualizarUsuarioValidador()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do usuário é obrigatório.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("Informe um e-mail válido.");
    }
}
