using Marketplace.Auth.Dominio.Enums;
using Marketplace.Auth.Utils.Helpers;

namespace Marketplace.Auth.Dominio.Entidades;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string? NomeFantasia { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public string Documento { get; private set; } = string.Empty;
    public DateOnly? DataNascimento { get; private set; }
    public string? Telefone { get; private set; }
    public ETipoPessoa TipoPessoa { get; private set; }
    public EUsuarioFuncao Funcao { get; private set; }
    public EUsuarioStatus Status { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }
    public string? TokenRedefinicaoSenha { get; private set; }
    public DateTime? TokenRedefinicaoSenhaExpiresIn { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public static Usuario Criar(
        string nome,
        string email,
        string senhaHash,
        string documento,
        ETipoPessoa tipoPessoa,
        EUsuarioFuncao funcao,
        string? nomeFantasia = null,
        DateOnly? dataNascimento = null,
        string? telefone = null)
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = nome.Trim(),
            NomeFantasia = nomeFantasia?.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            SenhaHash = senhaHash,
            Documento = DocumentoHelper.ApenasDigitos(documento),
            TipoPessoa = tipoPessoa,
            Funcao = funcao,
            DataNascimento = dataNascimento,
            Telefone = string.IsNullOrWhiteSpace(telefone) ? null : DocumentoHelper.ApenasDigitos(telefone),
            Status = EUsuarioStatus.Ativo,
            CriadoEm = DateTime.UtcNow
        };
    }

    public void Atualizar(string nome, string email, string? nomeFantasia, DateOnly? dataNascimento, string? telefone)
    {
        Nome = nome.Trim();
        NomeFantasia = nomeFantasia?.Trim();
        Email = email.Trim().ToLowerInvariant();
        DataNascimento = dataNascimento;
        Telefone = string.IsNullOrWhiteSpace(telefone) ? null : DocumentoHelper.ApenasDigitos(telefone);
        AtualizadoEm = DateTime.UtcNow;
    }

    public void AlterarSenha(string senhaHash)
    {
        SenhaHash = senhaHash;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void DefinirTokenRedefinicaoSenha(string token, DateTime expiresIn)
    {
        TokenRedefinicaoSenha = token;
        TokenRedefinicaoSenhaExpiresIn = expiresIn;
    }

    public void LimparTokenRedefinicaoSenha()
    {
        TokenRedefinicaoSenha = null;
        TokenRedefinicaoSenhaExpiresIn = null;
    }

    public void Inativar() => Status = EUsuarioStatus.Inativo;

    public void Suspender() => Status = EUsuarioStatus.Suspenso;
}
