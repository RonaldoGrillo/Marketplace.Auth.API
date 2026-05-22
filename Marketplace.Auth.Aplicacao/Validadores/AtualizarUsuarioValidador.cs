using FluentValidation;
using Marketplace.Auth.Aplicacao.UseCases.Usuarios;
using Marketplace.Auth.Utils.Helpers;

namespace Marketplace.Auth.Aplicacao.Validadores;

public class AtualizarUsuarioValidador : AbstractValidator<AtualizarUsuarioCommand>
{
    public AtualizarUsuarioValidador()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do usuário é obrigatório.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(200).WithMessage("O nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("Informe um e-mail válido.");

        RuleFor(x => x.NomeFantasia)
            .MaximumLength(200).WithMessage("O nome fantasia deve ter no máximo 200 caracteres.")
            .When(x => x.NomeFantasia is not null);

        RuleFor(x => x.Telefone)
            .Must(t => DocumentoHelper.ApenasDigitos(t!).Length >= 10)
            .WithMessage("O telefone deve ter pelo menos 10 dígitos (DDD + número).")
            .When(x => x.Telefone is not null);

        RuleFor(x => x.DataNascimento)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("A data de nascimento/fundação deve ser no passado.")
            .When(x => x.DataNascimento is not null);
    }
}
