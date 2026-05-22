using Marketplace.Auth.Dominio.Entidades;
using Marketplace.Auth.Utils.BancoDeDados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Auth.Repositorio.Persistencia.Configuracoes;

public class UsuarioConfiguracao : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuario");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("usu_id").HasColumnType(PostgreSqlTipo.Uuid.ParaSql());

        builder.Property(u => u.Nome).HasColumnName("usu_nome").HasColumnType(PostgreSqlTipo.CharacterVarying.ParaSql(200)).IsRequired();
        builder.Property(u => u.NomeFantasia).HasColumnName("usu_nomefantasia").HasColumnType(PostgreSqlTipo.CharacterVarying.ParaSql(200));
        builder.Property(u => u.Email).HasColumnName("usu_email").HasColumnType(PostgreSqlTipo.CharacterVarying.ParaSql(256)).IsRequired();
        builder.Property(u => u.SenhaHash).HasColumnName("usu_senhahash").HasColumnType(PostgreSqlTipo.CharacterVarying.ParaSql(60)).IsRequired();
        builder.Property(u => u.Documento).HasColumnName("usu_documento").HasColumnType(PostgreSqlTipo.CharacterVarying.ParaSql(14)).IsRequired();
        builder.Property(u => u.DataNascimento).HasColumnName("usu_datanascimento").HasColumnType(PostgreSqlTipo.Date.ParaSql());
        builder.Property(u => u.Telefone).HasColumnName("usu_telefone").HasColumnType(PostgreSqlTipo.CharacterVarying.ParaSql(20));
        builder.Property(u => u.TipoPessoa).HasColumnName("usu_tipopessoa").HasColumnType(PostgreSqlTipo.Smallint.ParaSql()).IsRequired();
        builder.Property(u => u.Funcao).HasColumnName("usu_funcao").HasColumnType(PostgreSqlTipo.Smallint.ParaSql()).IsRequired();
        builder.Property(u => u.Status).HasColumnName("usu_status").HasColumnType(PostgreSqlTipo.Smallint.ParaSql()).IsRequired();
        builder.Property(u => u.CriadoEm).HasColumnName("usu_criadoem").HasColumnType(PostgreSqlTipo.TimestampWithTimeZone.ParaSql()).IsRequired();
        builder.Property(u => u.AtualizadoEm).HasColumnName("usu_atualizadoem").HasColumnType(PostgreSqlTipo.TimestampWithTimeZone.ParaSql());
        builder.Property(u => u.TokenRedefinicaoSenha).HasColumnName("usu_token_redefinicaosenha").HasColumnType(PostgreSqlTipo.CharacterVarying.ParaSql(36));
        builder.Property(u => u.TokenRedefinicaoSenhaExpiresIn).HasColumnName("usu_tokenredefinicaosenhaexpiresin").HasColumnType(PostgreSqlTipo.TimestampWithTimeZone.ParaSql());

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Documento).IsUnique();

        builder.HasMany(u => u.RefreshTokens).WithOne(rt => rt.Usuario).HasForeignKey(rt => rt.UsuarioId).OnDelete(DeleteBehavior.Cascade);
    }
}
