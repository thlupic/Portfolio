using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project1.Controllers
{
    public class SetupController : Controller
    {
        private BLL.Manager manager {get; set;} //= new BLL.Manager();

        public ActionResult Index()
        {
            Core.serverSetup serverSetup = new Core.serverSetup();
            {
                serverSetup.server = Session["server"].ToString();
                serverSetup.port = Session["port"].ToString();
                serverSetup.username = Session["username"].ToString();
                serverSetup.password = Session["password"].ToString();
                serverSetup.DB = Session["DB"].ToString();
            }
            Models.SetupModel setupModel = new Models.SetupModel();
            setupModel.setupData = serverSetup;

            return View(setupModel);
        }

        [HttpPost]
        public ActionResult Index(Models.SetupModel setupModel)
        {
            if (setupModel.movieName != null)
            {
                this.manager = new BLL.Manager(setSessionData(setupModel.setupData));
                var result = manager.addMovie(setupModel.movieName);
                setupModel.result = result;
                if (result.Equals("Zapis uspješno dodan")) setupModel.movieName = "";
            }
            setSessionData(setupModel.setupData);
            return View(setupModel);
        }

        private Core.serverSetup setSessionData(Core.serverSetup serverSetup)
        {
            Session["server"] = serverSetup.server;
            Session["port"] = serverSetup.port;
            Session["username"] = serverSetup.username;
            Session["password"] = serverSetup.password;
            Session["DB"] = serverSetup.DB;

            return serverSetup;
        }

    }
}
