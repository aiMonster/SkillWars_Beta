using Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Account
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string NickName { get; set; }

        public UserInfo() { }

        public UserInfo(UserEntity entity)
        {
            Id = entity.Id;
            NickName = entity.NickName;
        }
    }
}
