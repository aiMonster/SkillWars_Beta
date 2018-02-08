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
        //public LobbieStatusTypes Status { get; set; }
        public DateTime CreationDate { get; set; }
        public int ExpectingSeconds { get; set; }
        public bool IsPrivate { get; set; } 
        
        public string Status { get; set; }
        public string Link { get; set; }

        public LobbieInfo() { }

        public LobbieInfo(LobbieEntity enitity)
        {
            Id = enitity.Id;
            Map = enitity.Map;
            Bet = enitity.Bet;
            AmountPlayers = enitity.AmountPlayers;
            switch(enitity.Status)
            {
                case LobbieStatusTypes.Canceled:
                    Status = "Отменено придурок!";
                    break;
                case LobbieStatusTypes.Expectation:
                    Status = "В ожидания начала пидор!";
                    break;
                case LobbieStatusTypes.Finished:
                    Status = "Уже закончилось идиот!";
                    break;
                case LobbieStatusTypes.Started:
                    Status = "Началось тормоз!";
                    break;
            }
            Link = @"http://localhost:4200/Lobbies/" + Id + "/players";
            //Status = enitity.Status;
            CreationDate = enitity.CreationDate;
            ExpectingSeconds = enitity.ExpectingSeconds;
            IsPrivate = enitity.IsPrivate;
        }

        //public List<TeamInfo> Teams { get; set; }
    }
}
