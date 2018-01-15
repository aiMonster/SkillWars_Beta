using Common.DTO.Account;
using Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Lobbie
{
    public class TeamInfo
    {
        public int Id { get; set; }

        public int LobbieId { get; set; }
        public List<UserDTO> Users { get; set; }

        public TeamInfo() { }

        public TeamInfo(TeamEntity entity, List<UserEntity> users)
        {
            Id = entity.Id;
            LobbieId = entity.LobbieId;
            Users = new List<UserDTO>();
            foreach(var u in users)
            {
                Users.Add(new UserDTO(u));
            }
            //entity.
        }
       
    }
}
