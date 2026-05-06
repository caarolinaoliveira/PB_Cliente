using PB.Cliente.Application.Requests.Clientes;
using FluentValidation;
using System;

namespace PB.Cliente.Application.Validators
{
    public class RegistrarClienteValidator : AbstractValidator<RegistrarClienteRequest>
    {
        public RegistrarClienteValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O email é obrigatório.")
                .EmailAddress().WithMessage("O email deve ser válido.");

            RuleFor(x => x.DataNascimento)
                .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
                .LessThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("A data de nascimento deve ser no passado.");

            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("O CPF é obrigatório.");
            
            RuleFor(x => x.Rg)
                .NotEmpty().WithMessage("O RG é obrigatório");
        }
    }
}