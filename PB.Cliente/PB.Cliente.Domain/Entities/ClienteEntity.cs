using PB.Cliente.Domain.Enums;
using PB.Cliente.Domain.Exceptions;


namespace PB.Cliente.Domain.Entities
{

    public class ClienteEntity : Entity
    {
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Cpf { get; private set; }
        public ClienteStatusEnum Status { get; private set; }
        public DateOnly DataNascimento { get; private set; }
        public string Telefone { get; private set; }

        protected ClienteEntity() { }

        public ClienteEntity(
            string nome,
            string email,
            string cpf,
            DateOnly dataNascimento,
            string telefone)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            DataNascimento = dataNascimento;
            Telefone = telefone;
            Ativar();
        }

        public void Ativar()
        {
            Status = ClienteStatusEnum.Ativo;
        }

        public void Suspender(string motivo)
        {
            if (Status == ClienteStatusEnum.Suspenso)
                throw new DomainException("Cliente já está bloqueado.");

            Status = ClienteStatusEnum.Suspenso;
        }
    }
}