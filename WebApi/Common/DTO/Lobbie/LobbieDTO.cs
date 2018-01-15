using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Lobbie
{
    public class LobbieDTO
    {        
        public string Map { get; set; }
        public double Bet { get; set; }
        public int AmountPlayers { get; set; }
        public int ExpectingSeconds { get; set; }

        public bool IsPrivate { get; set; }
        public string Password { get; set; }
    }
}
