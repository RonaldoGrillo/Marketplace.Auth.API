using Mapster;
using Marketplace.Auth.Aplicacao.DTOs;
using Marketplace.Auth.Dominio.Entidades;

namespace Marketplace.Auth.Aplicacao.Mapeamentos;

public class UsuarioPerfilMapeamento : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Usuario, UsuarioDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Nome, src => src.Nome)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Funcao, src => src.Funcao)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.CriadoEm, src => src.CriadoEm);
    }
}
