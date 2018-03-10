using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SkillArenaParser
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //sending login request
                WebRequest request = WebRequest.Create("https://www.myarena.ru/ajax.php?action=checklogin&ulogin=yevgenstrotsen&upassword=yevhen2015&gcode=&pcode=&ecode=&tcode=&capcode=");
                request.Method = "GET";
                WebResponse response = request.GetResponse();
                                
                string phpSessId = response.Headers.GetValues("Set-Cookie")
                                                   .Where(h => h.Contains("PHPSESSID"))
                                                   .FirstOrDefault()
                                                   .Split(';')[0]; 

                WebClient client = new WebClient();
                client.Headers.Add(HttpRequestHeader.Cookie, phpSessId);                
                client.QueryString.Add("m", "gamevds");
                client.QueryString.Add("p", "rcon");
                client.QueryString.Add("home", "7400");
                client.QueryString.Add("c", "players");

                //client.DownloadFile("https://www.myarena.ru/home.php", @"D:\localarena11.html");
                List<Player> players = Parse(client.DownloadString("https://www.myarena.ru/home.php"));

                foreach (var u in players)
                {
                    Console.WriteLine("\n" + new string('=', 30));
                    Console.WriteLine(u);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                
            }
            Console.ReadLine();
        }

        static List<Player> Parse(string html)
        {
            var parser = new HtmlParser();
            var document = parser.Parse(html);

            var playersTable = document.All.Where(m => m.Id == "playersTable").FirstOrDefault();              
            var rows = playersTable.QuerySelectorAll("tr").Skip(1);

            List<Player> players = new List<Player>();
            foreach (var row in rows)
            {
                Player player = new Player();              

                try
                {
                    var columns = row.QuerySelectorAll("td");

                    player.Id = row.Id;
                    player.SteamId = columns[1].TextContent;
                    player.Time = columns[3].TextContent;
                    player.Ping = columns[4].TextContent;
                    player.Name = columns[0].QuerySelector("b").TextContent;
                    player.Ip = columns[2].QuerySelector("a").GetAttribute("href")?.Split('=')[1];
                    
                
                    var img = columns[2].QuerySelector("img");
                    player.City = img?.GetAttribute("title")?.Split(':')[1];
                    player.Country = img?.GetAttribute("src")?.Split('/')[3]?.Split('.')[0];
                }
                catch
                {
                    Console.BackgroundColor = ConsoleColor.Cyan;
                }

                players.Add(player);                   
            }   

            return players;            
        }

        static void ParseByAgilityPack(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();           
            //doc.LoadHtml(File.ReadAllText(@"D:\localarena11.html"));
            doc.LoadHtml(html);
            doc.OptionDefaultStreamEncoding = Encoding.Unicode;
            Console.WriteLine("\n\n====== Loaded =====");

            //var table = doc.DocumentNode.SelectSingleNode("tbody");
            var playersTable = doc.GetElementbyId("playersTable");
            var rows = playersTable.SelectNodes("tr").Skip(1).ToList();
            Console.WriteLine("Rows - " + rows.Count);

            List<Player> players = new List<Player>();
            foreach(var row in rows)
            {
                Player player = new Player();
                var columns = row.SelectNodes("td");

                //Id
                player.Id = row.Id;
                //steam id
                player.SteamId = columns[1].InnerText;
                //time
                player.Time = columns[3].InnerText;
                //ping
                player.Ping = columns[4].InnerText;
                //name
                player.Name = columns[0].SelectSingleNode("b").InnerText;
                //Ip
                player.Ip = columns[2].SelectSingleNode("a").GetAttributeValue("href", "null=null").Split('=')[1];
                //Country/City               
                try
                {
                    var img = columns[2].SelectSingleNode("img");
                    var c = img.GetAttributeValue("title", "null:null").Split(':')[1];
                    Console.WriteLine(c);

                    player.City = c;
                    player.Country = img.GetAttributeValue("src", "i/f/p/c.p").Split('/')[3].Split('.')[0];
                }
                catch
                {
                    player.Country = "null";
                    player.City = "not selected";
                }
                players.Add(player);    
            }

            foreach (var u in players)
            {
                Console.WriteLine("\n" + new string('=', 30));
                Console.WriteLine(u);
            }
        }
}

    public class Player
    {
        public string Id { get; set; }//
        public string Name { get; set; }
        public string SteamId { get; set; }//
        public string Country { get; set; }
        public string City { get; set; }
        public string Ip { get; set; }
        public string Time { get; set; }//
        public string Ping { get; set; }//

        public override string ToString()
        {
            string result = "";
            result += "- Id: " + Id + "\n";
            result += "- Name: " + Name + "\n";
            result += "- SteamId: " + SteamId + "\n";
            result += "- Country: " + Country + "\n";
            result += "- City: " + City + "\n";
            result += "- Ip: " + Ip + "\n";
            result += "- Time: " + Time + "\n";
            result += "- Ping: " + Ping + "\n";
            return result;
        }
    }

}
