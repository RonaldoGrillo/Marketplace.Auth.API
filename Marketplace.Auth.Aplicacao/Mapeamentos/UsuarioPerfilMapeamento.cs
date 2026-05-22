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
            .Map(dest => dest.NomeFantasia, src => src.NomeFantasia)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Documento, src => src.Documento)
            .Map(dest => dest.DataNascimento, src => src.DataNascimento)
            .Map(dest => dest.Telefone, src => src.Telefone)
            .Map(dest => dest.TipoPessoa, src => src.TipoPessoa)
            .Map(dest => dest.Funcao, src => src.Funcao)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.CriadoEm, src => src.CriadoEm);
    }
}
