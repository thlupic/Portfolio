using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project1.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        private BLL.Manager manager { get; set; }

        public ActionResult Index()
        {
            manager = new BLL.Manager(setSessionData());
            List<Core.Movie> movies = manager.getAllMovies();
            Models.SearchResultModel searchResultModel = new Models.SearchResultModel();
            searchResultModel.SearchValueText = "svi";
            foreach (var item in movies)
            {
                searchResultModel.SearchResult.Add(item.name);
            }
            return View(searchResultModel);
        }

        [HttpPost]
        public ActionResult Index(Models.SearchResultModel searchResultModel)
        {
            manager = new BLL.Manager(setSessionData());
            var AndisTrue = searchResultModel.QueryAndValue;
            var searchType = searchResultModel.SearchType;
            var searchValue = searchResultModel.SearchValue;
            List<string> searchQuery = manager.divideQuery(searchValue);
            //string query1 = manager.buildQuerySelect(manager.divideQuery(searchValue), AndisTrue);
            //string query2 = manager.buildQueryExact2(manager.divideQuery(searchValue), AndisTrue);
            string query = manager.buildFullQuery(manager.divideQuery(searchValue), AndisTrue, searchResultModel.SearchType);
            string queryText = query;
            List<Core.Movie> movies = manager.getAllMovies();
            try
            {
                movies = manager.getResultsfromDB(query, searchQuery, AndisTrue);
            }
            catch (Exception e)
            {
                queryText = e.ToString();
            }
            searchResultModel.SearchValueText = manager.buildQuerySelect(manager.divideQuery(searchValue), AndisTrue);
            searchResultModel.SQLQuery = queryText;
            foreach (var item in movies)
            {
                searchResultModel.SearchResult.Add(item.name);
                searchResultModel.rank.Add(item.rank);
            }
            return View(searchResultModel);
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
