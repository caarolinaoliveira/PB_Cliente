
using PB.Cliente.Application.Requests.Clientes;
using PB.Cliente.Application.Responses.Clientes;

namespace PB.Cliente.Application.Interfaces
{
    public interface IClienteService
    {
        Task<ClienteResponse> RegistrarCliente(RegistrarClienteRequest request);
        Task<ClienteResponse> ObterClientePorId(Guid id);
    }
    
}