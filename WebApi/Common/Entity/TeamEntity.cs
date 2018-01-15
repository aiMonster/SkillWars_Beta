using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    public class TeamEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int LobbieId { get; set; }
        public LobbieEntity Lobbie { get; set; }

        public List<UserTeamEntity> UserTeams { get; set; }
        public TeamEntity()
        {
            UserTeams = new List<UserTeamEntity>();
        }

        public TeamEntity(int lobbieId)
        {
            UserTeams = new List<UserTeamEntity>();
            LobbieId = lobbieId;
        }

    }
}
