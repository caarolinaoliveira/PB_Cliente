using Microsoft.EntityFrameworkCore;
using PB.Cliente.Domain.Entities;
using PB.Cliente.Infrastructure.Mappings;

namespace PB.Cliente.Infrastructure.Context
{
    public class ClienteDbContext : DbContext
    {
        public ClienteDbContext(DbContextOptions<ClienteDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClienteEntity> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClienteDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
    
}