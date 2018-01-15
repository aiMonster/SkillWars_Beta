using Common.DTO.Lobbie;
using Common.Enums;
using Common.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    public class LobbieEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Map { get; set; }

        public double Bet { get; set; }

        public int AmountPlayers { get; set; }

        public LobbieStatusTypes Status { get; set; }

        public DateTime CreationDate { get; set; }

        public int ExpectingSeconds { get; set; }

        public bool IsPrivate { get; set; }

        public string Password { get; set; }

        public List<TeamEntity> Teams { get; set; }

        public LobbieEntity() { }

        public LobbieEntity(LobbieDTO lobbie)
        {
            Map = lobbie.Map;
            Bet = lobbie.Bet;
            AmountPlayers = lobbie.AmountPlayers;
            ExpectingSeconds = lobbie.ExpectingSeconds;
            IsPrivate = lobbie.IsPrivate;
            if(IsPrivate)
            {
                Password = TripleDESCryptHelper.Encript(lobbie.Password);
            }

            Status = LobbieStatusTypes.Expectation;
            CreationDate = DateTime.UtcNow;            
        }

    }
}
