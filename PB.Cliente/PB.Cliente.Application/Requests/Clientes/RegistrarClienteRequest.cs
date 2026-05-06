using System.ComponentModel.DataAnnotations;

namespace PB.Cliente.Application.Requests.Clientes
{
    public sealed record RegistrarClienteRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; init; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email informado é inválido.")]
        public string Email { get; init; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateOnly? DataNascimento { get; init; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        public string Cpf { get; init; }

        [Required(ErrorMessage = "O RG é obrigatório.")]
        public string Rg {get; init;}

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone(ErrorMessage = "O telefone informado é inválido.")]
        public string Telefone { get; init; }
    }
}