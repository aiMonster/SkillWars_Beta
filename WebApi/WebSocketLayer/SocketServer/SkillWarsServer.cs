using DataAccessLayer.Context;
using SuperSocket.SocketBase;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketLayer.General.Interfaces;

namespace WebSocketLayer.SocketServer
{
    public partial class SkillWarsServer: ISkillWarsServer
    {

        private WebSocketServer appServer;
        //private MSContext _context;

        public SkillWarsServer()
        {
            //_context = context;
            try
            {
                Console.WriteLine("Setuping WebSocketServer!");
                HardLogger.Logs.Add("Setuping WebSocketServer!");
                appServer = new WebSocketServer();
                if (!appServer.Setup(8000)) //Setup with listening port
                {
                    HardLogger.Logs.Add("Failed to startup");
                    return;
                }

                appServer.NewSessionConnected += new SessionHandler<WebSocketSession>(appServer_NewSessionConnected);
                appServer.SessionClosed += new SessionHandler<WebSocketSession, CloseReason>(appServer_SessionClosed);
                appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(appServer_NewMessageReceived);

                if (!appServer.Start())
                {
                    HardLogger.Logs.Add("Failed to start!");
                    return;
                }

                Console.WriteLine("WebSocketServer started!");
                HardLogger.Logs.Add("WebSocketServer started!");
            }
            catch(Exception ex)
            {
                HardLogger.Logs.Add(ex.Message);
            }
            
        }
    }
}
