using Common.DTO.Account;
using Common.Entity;
using Common.Enums;
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

        //public int LobbieId { get; set; }
        public TeamTypes TeamType { get; set; }
        public List<UserInfo> Users { get; set; }

        public TeamInfo() { }

        public TeamInfo(TeamEntity entity, List<UserEntity> users)
        {
            Id = entity.Id;
            TeamType = entity.Type;
            //LobbieId = entity.LobbieId;
            Users = new List<UserInfo>();
            foreach(var u in users)
            {
                Users.Add(new UserInfo(u));
            }
            //entity.
        }
       
    }
}
