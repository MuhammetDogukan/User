using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MassTransit;

namespace Application.MassTransitRabbiMq
{
    public class MassRabbitConsumer : IConsumer
    {
        public async Task GetMessageMassTransit(ConsumeContext<SendMailQ> context)
        {
            var message = context.Message;
            await SendMail(message.Body,message.To);
        }
        private async Task SendMail(string message, string to)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("UserApp@gmail.com"),
                Subject = "Welcome to the user system",
                Body = message
            };

            mailMessage.To.Add(to);
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
