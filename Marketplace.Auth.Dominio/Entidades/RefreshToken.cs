namespace Marketplace.Auth.Dominio.Entidades;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiraEm { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public bool Revogado { get; private set; }
    public Guid UsuarioId { get; private set; }
    public Usuario? Usuario { get; private set; }

    protected RefreshToken() { }

    public static RefreshToken Criar(string token, Guid usuarioId, DateTime expiraEm)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UsuarioId = usuarioId,
            ExpiraEm = expiraEm,
            CriadoEm = DateTime.UtcNow,
            Revogado = false
        };
    }

    public bool EstaValido() => !Revogado && ExpiraEm > DateTime.UtcNow;

    public void Revogar() => Revogado = true;
}
