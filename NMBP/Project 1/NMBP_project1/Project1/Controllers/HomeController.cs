using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project1.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Main/

        public ActionResult Index()
        {
            string server = "192.168.56.12";
            string port = "5432";
            string username = "postgres";
            string password = "reverse";
            string DB = "Movies";

            Session["server"] = server;
            Session["port"] = port;
            Session["username"] = username;
            Session["password"] = password;
            Session["DB"] = DB;

            //database.openConnection(server,port,username,password,DB);
            //List<Core.Movie> movies = database.getAllMovies();
            return View();
        }

    }
}
