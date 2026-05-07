using PB.Cliente.Application.Interfaces;
using System.Text.Json;
using RabbitMQ.Client;
using System.Text;

namespace PB.Cliente.Infrastructure.Messaging
{
    public class RabbitMQPublisher : IMessagePublisher
    {
        private readonly IConnection _connection;

        public RabbitMQPublisher(IConnection connection)
        {
            _connection = connection;
        }

        public async Task PublicarAsync<T>(T evento, string fila) where T : class
        {
            using var channel = _connection.CreateModel();

            channel.ConfirmSelect();
            var json = JsonSerializer.Serialize(evento);
            var body = Encoding.UTF8.GetBytes(json);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            channel.BasicPublish(
                exchange: "",
                routingKey: fila,
                basicProperties: props,
                body: body
            );

            var confirmado = channel.WaitForConfirms(timeout: TimeSpan.FromSeconds(5));
            if (!confirmado)
            {
                throw new Exception("Falha ao publicar mensagem no RabbitMQ.");
            }
            
            await Task.CompletedTask;
        }
    }
}