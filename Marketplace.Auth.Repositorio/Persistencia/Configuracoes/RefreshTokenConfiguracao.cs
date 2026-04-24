using Marketplace.Auth.Dominio.Entidades;
using Marketplace.Auth.Utils.BancoDeDados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Auth.Repositorio.Persistencia.Configuracoes;

public class RefreshTokenConfiguracao : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refreshtoken");

        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id).HasColumnName("rtk_id").HasColumnType(PostgreSqlTipo.Uuid.ParaSql());

        builder.Property(rt => rt.Token).HasColumnName("rtk_token").HasColumnType(PostgreSqlTipo.CharacterVarying.ParaSql(100)).IsRequired();
        builder.Property(rt => rt.ExpiresIn).HasColumnName("rtk_expiresin").HasColumnType(PostgreSqlTipo.TimestampWithTimeZone.ParaSql()).IsRequired();
        builder.Property(rt => rt.CriadoEm).HasColumnName("rtk_criadoem").HasColumnType(PostgreSqlTipo.TimestampWithTimeZone.ParaSql()).IsRequired();
        builder.Property(rt => rt.Revogado).HasColumnName("rtk_revogado").HasColumnType(PostgreSqlTipo.Boolean.ParaSql()).IsRequired();
        builder.Property(rt => rt.UsuarioId).HasColumnName("rtk_usu_id").HasColumnType(PostgreSqlTipo.Uuid.ParaSql()).IsRequired();

        builder.HasIndex(rt => rt.Token).IsUnique();
    }
}