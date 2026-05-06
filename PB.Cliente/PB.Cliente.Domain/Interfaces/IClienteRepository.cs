using System.Linq.Expressions;
using PB.Cliente.Domain.Entities;

namespace PB.Cliente.Domain.Interfaces
{
    public interface IClienteRepository : IRepository<ClienteEntity>
    {
        Task<ClienteEntity?> ObterPorCpfAsync(string cpf);
        Task<ClienteEntity?> ObterPorIdAsync(Guid id);
        Task<ClienteEntity?> ObterPorEmailAsync(string email);
    }
}