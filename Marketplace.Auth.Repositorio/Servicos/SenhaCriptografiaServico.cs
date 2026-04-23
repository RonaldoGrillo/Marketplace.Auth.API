using Marketplace.Auth.Aplicacao.Interfaces;

namespace Marketplace.Auth.Repositorio.Servicos;

public class SenhaCriptografiaServico : ISenhaCriptografiaServico
{
    public string Criptografar(string senha) => BCrypt.Net.BCrypt.HashPassword(senha);
    public bool Verificar(string senha, string hash) => BCrypt.Net.BCrypt.Verify(senha, hash);
}
