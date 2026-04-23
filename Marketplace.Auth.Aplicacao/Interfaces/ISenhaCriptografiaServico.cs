namespace Marketplace.Auth.Aplicacao.Interfaces;

public interface ISenhaCriptografiaServico
{
    string Criptografar(string senha);
    bool Verificar(string senha, string hash);
}
