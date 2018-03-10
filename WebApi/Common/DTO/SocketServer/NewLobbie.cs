using Common.DTO.Lobbie;
using Common.Entity;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.SocketServer
{
    public class NewLobbie : LobbieInfo
    {
        public MessageTypes MessageType { get; set; } = MessageTypes.NewLobbie;

        public NewLobbie() : base() { }

        public NewLobbie(LobbieEntity entity) : base(entity) { }
        //public NewLobbie(LobbieInfo info)
        //{

        //    Id = info.Id;
        //    Map = info.Map;
        //    Bet = info.Bet;
        //    AmountPlayers = info.AmountPlayers;
        //    CreationDate = info.CreationDate;
        //    ExpectingSeconds = info.ExpectingSeconds;
        //    IsPrivate = info.IsPrivate;
        //    Status = info.Status;
        //    Link = info.Link;
        //}
    }
}
