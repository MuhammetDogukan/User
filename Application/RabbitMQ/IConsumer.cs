using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RabbitMQ
{
    public interface IConsumer
    {
        Task GetMessageFromQueue(string message, string toMail);
    }
}
