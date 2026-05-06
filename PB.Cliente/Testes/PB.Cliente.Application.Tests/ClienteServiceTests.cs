using PB.Cliente.Application.Services;
using PB.Cliente.Application.Interfaces;
using PB.Cliente.Application.Requests.Clientes;
using PB.Cliente.Application.Events;
using PB.Cliente.Domain.Interfaces;
using PB.Cliente.Domain.Entities;
using PB.Cliente.Domain.Exceptions; 
using FluentAssertions;
using Moq;
using Xunit;

namespace PB.Cliente.Application.Tests;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IMessagePublisher> _messagePublisherMock;
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _messagePublisherMock = new Mock<IMessagePublisher>();
        _service = new ClienteService(_clienteRepositoryMock.Object, _messagePublisherMock.Object);
    }

    [Fact]
    public async Task RegistrarClienteAsync_DeveCriarClienteComSucesso()
    {
        //Arrange
        var request = new RegistrarClienteRequest
        {
            Nome = "Cliente Service Teste",
            Email = "clienteservice@teste.com",
            DataNascimento = new DateOnly(2006, 06, 01),
            Cpf = "12345645612",
            Rg = "144121750",
            Telefone = "41999999999"
        };
        
        _clienteRepositoryMock.Setup(repo => repo
            .ObterPorCpfAsync(request.Cpf))
            .ReturnsAsync((ClienteEntity)null);
        
        _clienteRepositoryMock
            .Setup(r => r.ObterPorEmailAsync(request.Email))
            .ReturnsAsync((ClienteEntity?)null);
        
        _messagePublisherMock
            .Setup(p => p.PublicarAsync(It.IsAny<ClienteCadastradoEvent>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        //Act 
        var result = await _service.RegistrarCliente(request);

        //Assert
        result.Should().NotBeNull();
        result.Nome.Should().Be(request.Nome);
        result.Email.Should().Be(request.Email);
        result.Cpf.Should().Be(request.Cpf);
        result.Status.Should().Be("Ativo");

        _clienteRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<ClienteEntity>()), Times.Once);
        _messagePublisherMock.Verify(p => p.PublicarAsync(It.Is<ClienteCadastradoEvent>(e => e.Cpf == request.Cpf && e.Nome == request.Nome), "cliente.cadastrado"), Times.Once);

    }

    [Fact]
    public async Task RegistrarClienteAsync_ClienteComCpfExistente_DeveLancarConflictException()
    {
        // Arrange
        var request = new RegistrarClienteRequest
        {
            Nome = "Cliente Service Teste",
            Email = "clienteservice@teste.com",
            DataNascimento = new DateOnly(2006, 06, 01),
            Cpf = "12345645612",
            Telefone = "41999999999"
        };

        var clienteExistenteMock = Mock.Of<ClienteEntity>();

        _clienteRepositoryMock
            .Setup(repo => repo.ObterPorCpfAsync(request.Cpf))
            .ReturnsAsync(clienteExistenteMock);

        // Act
        Func<Task> act = async () => await _service.RegistrarCliente(request);

        // Assert
        await act
            .Should()
            .ThrowAsync<ConflictException>()
            .WithMessage("CPF já cadastrado.");

        _clienteRepositoryMock.Verify(
            r => r.AdicionarAsync(It.IsAny<ClienteEntity>()),
            Times.Never
        );

        _messagePublisherMock.Verify(
            p => p.PublicarAsync(It.IsAny<ClienteCadastradoEvent>(), It.IsAny<string>()),
            Times.Never
        );
    }

    [Fact]
    public async Task RegistrarClienteAsync_ClienteComEmailExistente_DeveLancarConflictException()
    {
        // Arrange
        var request = new RegistrarClienteRequest
        {
            Nome = "Cliente Service Teste",
            Email = "clienteservice@teste.com",
            DataNascimento = new DateOnly(2006, 06, 01),
            Cpf = "12345645612",
            Telefone = "41999999999"
        };

        var clienteExistenteMock = Mock.Of<ClienteEntity>();

        _clienteRepositoryMock
            .Setup(repo => repo.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(clienteExistenteMock);

        // Act
        Func<Task> act = async () => await _service.RegistrarCliente(request);

        // Assert
        await act
            .Should()
            .ThrowAsync<ConflictException>()
            .WithMessage("Email já cadastrado.");

        _clienteRepositoryMock.Verify(
            r => r.AdicionarAsync(It.IsAny<ClienteEntity>()),
            Times.Never
        );

        _messagePublisherMock.Verify(
            p => p.PublicarAsync(It.IsAny<ClienteCadastradoEvent>(), It.IsAny<string>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ObterClientePorId_DeveRetornarClienteExistente()
    {
        // Arrange
        var clienteId = Guid.NewGuid();

        var clienteReal = new ClienteEntity(
            "Carolina Oliveira",
            "carolina@teste.com",
            "12345678901",
            new DateOnly(1999, 03, 15),
            "41999999999"
        );

        _clienteRepositoryMock
            .Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(clienteReal);

        // Act
        var result = await _service.ObterClientePorId(clienteReal.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Nome.Should().Be("Carolina Oliveira");
        result.Email.Should().Be("carolina@teste.com");
        result.Cpf.Should().Be("12345678901");

        _clienteRepositoryMock.Verify(r => r.ObterPorIdAsync(clienteReal.Id), Times.Once);
    }
}