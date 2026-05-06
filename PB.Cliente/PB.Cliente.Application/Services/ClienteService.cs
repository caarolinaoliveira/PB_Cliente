using PB.Cliente.Application.Interfaces;
using PB.Cliente.Application.Requests.Clientes;
using PB.Cliente.Application.Responses.Clientes;
using PB.Cliente.Application.Events;
using PB.Cliente.Domain.Entities;
using PB.Cliente.Domain.Interfaces;
using PB.Cliente.Domain.Exceptions;

namespace PB.Cliente.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMessagePublisher _messagePublisher;

        public ClienteService(IClienteRepository clienteRepository, IMessagePublisher messagePublisher)
        {
            _clienteRepository = clienteRepository;
            _messagePublisher = messagePublisher;
        }

        public async Task<ClienteResponse> RegistrarCliente(RegistrarClienteRequest request)
        {
            if (await _clienteRepository.ObterPorCpfAsync(request.Cpf) != null)
            {
                throw new ConflictException("CPF já cadastrado.");
            }

            if (await _clienteRepository.ObterPorEmailAsync(request.Email) != null) 
            {
                throw new ConflictException("Email já cadastrado.");
            }

            var cliente = new ClienteEntity(
                request.Nome,
                request.Email,
                request.Cpf,
                request.DataNascimento.Value,
                request.Telefone
            );

            await _clienteRepository.AdicionarAsync(cliente);

            var evento = new ClienteCadastradoEvent
            {
                ClienteId = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email,
                Cpf = cliente.Cpf,
                DataNascimento = cliente.DataNascimento,
                Telefone = cliente.Telefone
            };

            await _messagePublisher.PublicarAsync(evento, "cliente.cadastrado");

            return new ClienteResponse
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email,
                Cpf = cliente.Cpf,
                DataNascimento = cliente.DataNascimento,
                Telefone = cliente.Telefone,
                Status = cliente.Status.ToString()
            };
        }

        public async Task<ClienteResponse?> ObterClientePorId(Guid id)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null)
                throw new NotFoundException("Cliente não encontrado.");

            return new ClienteResponse
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email,
                Cpf = cliente.Cpf,
                DataNascimento = cliente.DataNascimento,
                Telefone = cliente.Telefone,
                Status = cliente.Status.ToString()
            };
        }
    }
}