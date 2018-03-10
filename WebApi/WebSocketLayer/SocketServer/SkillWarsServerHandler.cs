using Newtonsoft.Json;
using SuperSocket.SocketBase;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketLayer.SocketServer
{
    public partial class SkillWarsServer
    {
        private async void appServer_NewMessageReceived(WebSocketSession session, string message)
        {
            Console.WriteLine("Got new message");
            //var baseMessage = JsonConvert.DeserializeObject<BaseMessage>(message);
            //if (baseMessage.Type == NotificationTypes.RandomGameRequest)
            //{
            //    Console.WriteLine("New random user request");
            //    await RandomUserRequest(session);
            //}
            //else if (baseMessage.Type == NotificationTypes.CreateGame)
            //{
            //    Console.WriteLine("Created new game with friend");
            //    await CreateGame(session);
            //}
            //else if (baseMessage.Type == NotificationTypes.ParticipateGame)
            //{
            //    Console.WriteLine("User wants to participate game");
            //    await ParticipateGame(session, JsonConvert.DeserializeObject<GameWithFriendRequest>(message));
            //}
            //else if (baseMessage.Type == NotificationTypes.ShipsPlaced)
            //{
            //    Console.WriteLine("Someone ships are placed");
            //    await ShipsPlaced(session);
            //}
            //else if (baseMessage.Type == NotificationTypes.MoveMade)
            //{
            //    Console.WriteLine("Someone made move");
            //    await MoveMade(session, JsonConvert.DeserializeObject<MoveCoordinates>(message));
            //}
            //else if (baseMessage.Type == NotificationTypes.MoveChecked)
            //{
            //    Console.WriteLine("We checked move and sent responses for both users");
            //    await CheckMove(session, JsonConvert.DeserializeObject<ShotResult>(message));
            //}
        }

        private void appServer_NewSessionConnected(WebSocketSession session)
        {
            Console.WriteLine("New session connected! Sessions counter: " + appServer.SessionCount);
        }

        private void appServer_SessionClosed(WebSocketSession session, CloseReason value)
        {
            
            Console.WriteLine();
            Console.WriteLine("Client disconnected! Sessions counter: " + appServer.SessionCount);
        }

        public void SendMessageForAll(object message)
        {
            int counter = 0;
            foreach(var session in appServer.GetAllSessions())
            {
                session.Send(JsonConvert.SerializeObject(message));
                counter++;
            }
            Console.WriteLine("Message was sent for {0} users", counter);
            HardLogger.Logs.Add("Message was sent for " + counter + " users");
        }
    }
}
