using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntercasaWebSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Success()
        {

            return View();
        }

        [HttpGet]
        public string Shit(Info inf)
        {
            var query = Request.QueryString;
            InfoList.Str = Request.RawUrl;
            //if(query["ik_co_id"].ToString() == "")
            //{

            //}
            //InfoList.Collection.Add(a);
            //InfoList.Infos.Add(inf);
            //InfoList.Strings.Add(Request.RawUrl);
            return "nice";
        }

        public string GetInfo()
        {
            //Request.
            string result = "";
            foreach(var inf in InfoList.Collection)
            {
               foreach(var c in inf.AllKeys)
               {
                    result += c + " - " + inf[c];
               }
            }
            return InfoList.Str;
        }
     
    }

    

    public static class InfoList
    {
        public static string Str = "";
        public static List<NameValueCollection> Collection = new List<NameValueCollection>();
        public static List<string> Strings = new List<string>();
        public static List<Info> Infos = new List<Info>() { new Info() {ik_act = "shit" }};
    }


    public class Info
    {
        public string ik_co_id { get; set; }
        public string ik_pm_no { get; set; }
        public string ik_desc { get; set; }
        public string ik_pw_via { get; set; }
        public string ik_am { get; set; }
        public string ik_cur { get; set; }
        public string ik_act { get; set; }

        public override string ToString()
        {
            return ik_act + "-" + ik_co_id + "-" + ik_cur;
        }
        //public string ik_co_id { get; set; }
        //public string ik_co_id { get; set; }
        //public string ik_co_id { get; set; }
    }
}