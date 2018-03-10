using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.SocketServer
{
    public class RemovedLobbie
    {
        public MessageTypes MessageType { get; set; } = MessageTypes.RemovedLobbie;
        public int LobbieId { get; set; }
    }
}
