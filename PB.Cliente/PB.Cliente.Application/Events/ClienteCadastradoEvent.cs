namespace PB.Cliente.Application.Events
{
    public class ClienteCadastradoEvent
    {
        public Guid ClienteId { get; init; }
        public string Nome { get; init; }
        public string Email { get; init; }
        public string Cpf { get; init; }
        public string Rg {get; init;}
        public DateOnly DataNascimento { get; init; }
        public string Telefone { get; init; }
        public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
    }
}