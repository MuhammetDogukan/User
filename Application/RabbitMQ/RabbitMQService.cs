using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RabbitMQ
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly string _queueName = "mail_queue";

        public RabbitMQService()
        {
            _connectionFactory = new ConnectionFactory
            {
                // RabbitMQ sunucu bağlantı bilgilerini burada ayarlayın
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
        }

        public void SendMessage(string message)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
                }
            }
        }
    }
}
