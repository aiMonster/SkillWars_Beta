using Common.Entity;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Lobbie
{
    public class LobbieInfo
    {
        public int Id { get; set; }
        public string Map { get; set; }
        public double Bet { get; set; }
        public int AmountPlayers { get; set; }
        public LobbieStatusTypes Status { get; set; }
        public DateTime CreationDate { get; set; }
        public int ExpectingSeconds { get; set; }
        public bool IsPrivate { get; set; } 
        
        public LobbieInfo() { }

        public LobbieInfo(LobbieEntity enitity)
        {
            Id = enitity.Id;
            Map = enitity.Map;
            Bet = enitity.Bet;
            AmountPlayers = enitity.AmountPlayers;
            Status = enitity.Status;
            CreationDate = enitity.CreationDate;
            ExpectingSeconds = enitity.ExpectingSeconds;
            IsPrivate = enitity.IsPrivate;
        }

        //public List<TeamInfo> Teams { get; set; }
    }
}
