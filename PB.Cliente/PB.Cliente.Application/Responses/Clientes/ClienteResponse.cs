using System.ComponentModel.DataAnnotations;

namespace PB.Cliente.Application.Responses.Clientes
{
    
    public sealed record ClienteResponse
    {
        public Guid Id { get; init; }
        public string Nome { get; init; }
        public string Email { get; init; }
        public string Cpf { get; init; }
        public string Rg { get; init;}
        public string Telefone { get; init; }
        public DateOnly DataNascimento { get; init; }
        public string Status { get; init; }
    }

}