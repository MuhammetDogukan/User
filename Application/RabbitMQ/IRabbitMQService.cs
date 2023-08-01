using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RabbitMQ
{
    public interface IRabbitMQService
    {
        public void SendMessage(string message);
    }
}
