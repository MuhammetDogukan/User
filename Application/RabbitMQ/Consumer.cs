using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Application.RabbitMQ
{
    public class Consumer : BackgroundService
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly string _queueName = "mail_queue";

        public Consumer()
        {
            _connectionFactory = new ConnectionFactory
            {
                // RabbitMQ sunucu bağlantı bilgilerini burada ayarlayın
                HostName = "my-rabbitmq",
                UserName = "guest",
                Password = "guest"
            };
        }

        public async Task GetMessageFromQueue(string message, string toMail)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    var consumer = new EventingBasicConsumer(channel);
                    
                    consumer.Received += async (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        message = Encoding.UTF8.GetString(body);
                        //await SendMail(message, toMail);
                    };
                    channel.BasicConsume(queue: _queueName,
                                         autoAck: true,
                                         consumer: consumer);


                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += async (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        //await SendMail(message);
                    };
                    channel.BasicConsume(queue: _queueName,
                                         autoAck: true,
                                         consumer: consumer);


                }
            }
        }

        private async Task SendMail(string message)
        {
            string[] messages = message.Split(".");
            var mailMessage = new MailMessage
            {
                From = new MailAddress("UserApp@gmail.com"),
                Subject = "Welcome to the user system",
                Body = messages[0]
            };

            mailMessage.To.Add(messages[1]);
            using (var smtpClient = new SmtpClient("aaa@gmail.com", 25))
            {
                smtpClient.Credentials = new NetworkCredential("Dogukan", "1234");
                smtpClient.EnableSsl = true;

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (SmtpException ex)
                {

                    Console.WriteLine("An error occurred while sending mail: " + ex.Message);
                }
            }
        }
    }
}
