using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Lobbie
{
    public class TeamDTO
    {
        public int LobbieId { get; set; }
        public List<TeamInfo> Teams { get; set; }

        public TeamDTO()
        {
            Teams = new List<TeamInfo>();
        }

        
    }
}
