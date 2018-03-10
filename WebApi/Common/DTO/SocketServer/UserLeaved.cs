using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.SocketServer
{
    public class UserLeaved
    {
        public MessageTypes MessageType { get; set; } = MessageTypes.UserLeaved;
        public int UserId { get; set; }
        public int LobbieId { get; set; }

    }
}
