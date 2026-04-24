namespace Marketplace.Auth.Dominio.Entidades;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresIn { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public bool Revogado { get; private set; }
    public Guid UsuarioId { get; private set; }
    public virtual Usuario? Usuario { get; private set; }

    protected RefreshToken() { }

    public static RefreshToken Criar(string token, Guid usuarioId, DateTime expiresIn)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UsuarioId = usuarioId,
            ExpiresIn = expiresIn,
            CriadoEm = DateTime.UtcNow,
            Revogado = false
        };
    }

    public bool EstaValido() => !Revogado && ExpiresIn > DateTime.UtcNow;

    public void Revogar() => Revogado = true;
}
