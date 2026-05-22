using FluentValidation;
using Marketplace.Auth.Aplicacao.UseCases.Usuarios.CriarUsuario;
using Marketplace.Auth.Dominio.Enums;
using Marketplace.Auth.Utils.Helpers;

namespace Marketplace.Auth.Aplicacao.Validadores;

public class CriarUsuarioValidador : AbstractValidator<CriarUsuarioCommand>
{
    public CriarUsuarioValidador()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(200).WithMessage("O nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .Must(RegexHelper.ValidarEmail).WithMessage("O e-mail informado é inválido.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter pelo menos 8 caracteres.")
            .Matches(@"[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
            .Matches(@"[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
            .Matches(@"\d").WithMessage("A senha deve conter pelo menos um número.");

        RuleFor(x => x.Documento)
            .NotEmpty().WithMessage("O documento é obrigatório.")
            .Must((cmd, doc) => ValidarDocumento(doc, cmd.TipoPessoa))
            .WithMessage(cmd => cmd.TipoPessoa == ETipoPessoa.PessoaFisica
                ? "CPF inválido."
                : "CNPJ inválido.");

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

    private static bool ValidarDocumento(string documento, ETipoPessoa tipo) =>
        tipo == ETipoPessoa.PessoaFisica
            ? DocumentoHelper.ValidarCpf(documento)
            : DocumentoHelper.ValidarCnpj(documento);
}
