using Marketplace.Auth.Dominio.Enums;

namespace Marketplace.Auth.Dominio.Entidades;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public EUsuarioFuncao Funcao { get; private set; }
    public EUsuarioStatus Status { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }
    public string? TokenRedefinicaoSenha { get; private set; }
    public DateTime? TokenRedefinicaoSenhaExpiresIn { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    protected Usuario() { }

    public static Usuario Criar(string nome, string email, string senhaHash, EUsuarioFuncao funcao)
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Email = email.ToLowerInvariant(),
            SenhaHash = senhaHash,
            Funcao = funcao,
            Status = EUsuarioStatus.Ativo,
            CriadoEm = DateTime.UtcNow
        };
    }

    public void Atualizar(string nome, string email)
    {
        Nome = nome;
        Email = email.ToLowerInvariant();
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

    public void Desativar() => Status = EUsuarioStatus.Inativo;
}
