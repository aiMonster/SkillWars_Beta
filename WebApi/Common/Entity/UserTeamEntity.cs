using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    public class UserTeamEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }
        public UserEntity User { get; set; }

        public int TeamId { get; set; }
        public TeamEntity Team { get; set; }

        public int LobbieId { get; set; }
        //public LobbieEntity Lobbie { get; set; }
    }
}
