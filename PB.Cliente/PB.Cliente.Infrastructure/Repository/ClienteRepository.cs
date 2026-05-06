using System.Linq.Expressions;
using PB.Cliente.Domain.Entities;
using PB.Cliente.Domain.Interfaces;
using PB.Cliente.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace PB.Cliente.Infrastructure.Repository
{
    public class ClienteRepository : Repository<ClienteEntity>, IClienteRepository
    {
        public ClienteRepository(ClienteDbContext db) : base(db)
        {
        }

        public async Task<ClienteEntity> ObterPorCpfAsync(string cpf)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Cpf == cpf);
        }

        public async Task<ClienteEntity> ObterPorEmailAsync(string email)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Email == email);
        }
    }
}