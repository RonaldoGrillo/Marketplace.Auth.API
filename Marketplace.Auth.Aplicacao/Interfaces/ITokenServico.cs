using Marketplace.Auth.Dominio.Entidades;

namespace Marketplace.Auth.Aplicacao.Interfaces;

public interface ITokenServico
{
    string GerarAccessToken(Usuario usuario);
    RefreshToken GerarRefreshToken(Guid usuarioId);
    Guid? ObterUsuarioIdDoToken(string token);
}
