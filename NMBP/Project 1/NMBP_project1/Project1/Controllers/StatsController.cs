using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project1.Controllers
{
    public class StatsController : Controller
    {
        private BLL.Manager manager { get; set; } //= new BLL.Manager();

        public ActionResult Index()
        {
            Models.StatsModel analyseModel = new Models.StatsModel();
            analyseModel.startDate = DateTime.Today;
            analyseModel.endDate = DateTime.Today;
            return View(analyseModel);
        }

        [HttpPost]
        public ActionResult Index(Models.StatsModel analyseModel)
        {
            manager = new BLL.Manager(setSessionData());
            analyseModel.analyseResults = manager.getStatistics(analyseModel.startDate, analyseModel.endDate, analyseModel.isByHour);
            analyseModel.startDate = analyseModel.startDate;
            analyseModel.endDate = analyseModel.endDate;
            return View(analyseModel);
        }

        private Core.serverSetup setSessionData()
        {
            Core.serverSetup serverSetup = new Core.serverSetup();
            serverSetup.server = Session["server"].ToString();
            serverSetup.port = Session["port"].ToString();
            serverSetup.username = Session["username"].ToString();
            serverSetup.password = Session["password"].ToString();
            serverSetup.DB = Session["DB"].ToString();

            return serverSetup;
        }
    }
}
