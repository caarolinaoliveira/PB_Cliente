using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PB.Cliente.Domain.Entities;

namespace PB.Cliente.Infrastructure.Mappings
{
    public class ClienteMapping  : IEntityTypeConfiguration<ClienteEntity>
    {
        public void Configure(EntityTypeBuilder<ClienteEntity> builder)
        {
            builder.ToTable("Clientes");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(c => c.Cpf)
                .IsRequired()
                .HasMaxLength(11);
                
            builder.HasIndex(c => c.Cpf)
                .IsUnique();

            builder.Property(c => c.Status)
                .IsRequired();

            builder.Property(u => u.DataNascimento)
                .IsRequired()
                .HasColumnType("date") 
                .HasConversion(
                    d => d.ToDateTime(TimeOnly.MinValue),
                    d => DateOnly.FromDateTime(d)
                );

            builder.Property(c => c.Telefone)
                .IsRequired()
                .HasMaxLength(15);
        }
    }
    
}