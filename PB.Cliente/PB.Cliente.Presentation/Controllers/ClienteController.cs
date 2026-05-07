using Microsoft.AspNetCore.Mvc;
using PB.Cliente.Application.Interfaces;
using PB.Cliente.Application.Services;
using PB.Cliente.Application.Requests.Clientes;
using PB.Cliente.Application.Responses.Clientes;
using System.Net;
using PB.Cliente.Application.Validators;

namespace PB.Cliente.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/clientes")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpPost("cadastrar")]
        [ProducesResponseType(typeof(ClienteResponse), (int)HttpStatusCode.Created)]        
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RegistrarCliente(RegistrarClienteRequest request)
        {
            var validator = new RegistrarClienteValidator();
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var response = await _clienteService.RegistrarCliente(request);
            return CreatedAtAction(nameof(RegistrarCliente), new { id = response.Id }, response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClienteResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ObterClientePorId(Guid id)
        {
            var cliente = await _clienteService.ObterClientePorId(id);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }
    }
    
}