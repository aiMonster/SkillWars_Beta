using Common.DTO.Account;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.SocketServer
{
    public class UserParticipated
    {
        public MessageTypes MessageType { get; set; } = MessageTypes.UserParticipated;

        public int UserId { get; set; }
        public string NickName { get; set; }

        public int LobbieId { get; set; }
        public TeamTypes TeamType { get; set; }
        

    }
}
