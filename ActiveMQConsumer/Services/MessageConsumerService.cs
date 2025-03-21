using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

namespace ActiveMQConsumer.Services
{
    public class MessageConsumerService : BackgroundService
    {
        private IConnection _connection;
        private Apache.NMS.ISession _session;
        private IMessageConsumer _consumer;
        public static ConcurrentQueue<string> Messages {get; } = new();

        public MessageConsumerService()
        {
            var factory = new ConnectionFactory("tcp://localhost:61616");
            _connection = factory.CreateConnection();
            _connection.Start();
            _session = _connection.CreateSession();
            IDestination queue = _session.GetQueue("messages.queue");
            _consumer = _session.CreateConsumer(queue);
            _connection.Start();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Iniciando servicio consumer");
            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        IMessage message = _consumer.Receive(TimeSpan.FromSeconds(1));
                        if (message is ITextMessage textMessage)
                        {
                            Console.WriteLine($"Mensaje recibido: {textMessage.Text}"); // Muestra el mensaje en consola
                            Messages.Enqueue(textMessage.Text);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}"); // Captura errores
                        ResetConnection();
                    }
                }
            }, stoppingToken);
        }

        private void ResetConnection()
        {
            _consumer?.Close();
            _session?.Close();
            _connection?.Close();

            //Recrear conexion
            var factory = new ConnectionFactory("tcp://localhost:61616");
            _connection = factory.CreateConnection();
            _connection.Start();
            _session = _connection.CreateSession();
            IDestination queue = _session.GetQueue("messages.queue");
            _consumer = _session.CreateConsumer(queue);
            
        }

        public override void Dispose()
        {
            _consumer.Close();
            _session.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}