using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketLayer.General.Interfaces
{
    public interface ISkillWarsServer
    {
        void SendMessageForAll(object message);
    }
}
