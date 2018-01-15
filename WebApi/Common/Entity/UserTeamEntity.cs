using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    public class UserTeamEntity
    {
        public int UserId { get; set; }
        public UserEntity User { get; set; }

        public int TeamId { get; set; }
        public TeamEntity Team { get; set; }
    }
}
