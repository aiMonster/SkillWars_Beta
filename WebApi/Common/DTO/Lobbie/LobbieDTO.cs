using Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Lobbie
{
    public class LobbieDTO
    {        
        [Required]
        public string Map { get; set; }

        [Required]
        [Bet]
        public double Bet { get; set; }

        public int AmountPlayers { get; set; }
        public int ExpectingSeconds { get; set; }

        public bool IsPrivate { get; set; }
        public string Password { get; set; }
    }
}
