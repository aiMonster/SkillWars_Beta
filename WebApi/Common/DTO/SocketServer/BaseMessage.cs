using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.SocketServer
{
    public class BaseMessage
    {
        public MessageTypes MessageType { get; set; }

        public BaseMessage() { }

        public BaseMessage(MessageTypes type )
        {
            MessageType = type;
        }
    }
}
