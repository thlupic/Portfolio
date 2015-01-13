using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using System.IO;

namespace Project_2.Controllers
{
    public class HomeController : Controller
    {
        private static Manager manager = new Manager();

        public ActionResult Index()
        {
            Models.NewsFeedVm model = new Models.NewsFeedVm();
            int n = 10;
            if (Session["n"] == null)
            {
                Session["n"] = 10;
            }
            else
            {
                n = Convert.ToInt32(Session["n"].ToString());
            }
            model = Models.NewsHelper.MappAllNewsToVm(manager.getAllNews(n));            
            return View(model);
        }

        /* Adding comments to news */
        [HttpPost]
        public ActionResult Index(int newsID, string commentText)
        {
            manager.addComment(newsID, commentText);
            Models.NewsFeedVm model = new Models.NewsFeedVm();
            int n = Convert.ToInt32(Session["n"].ToString());
            model = Models.NewsHelper.MappAllNewsToVm(manager.getAllNews(n));
            return View(model);
        }

        public ActionResult AddNews()
        {
            var model = new Models.NewsVm();
            return View(model);
        }

        public ActionResult ArticlesNumber()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ArticlesNumber(string number)
        {
            //Models.NewsFeedVm model = new Models.NewsFeedVm();
            Session["n"] = number;
            //Session["n"] = Convert.ToInt32(number);
            //var model = Models.NewsHelper.MappAllNewsToVm(manager.getAllNews(Convert.ToInt32(number)));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddNews(Models.NewsVm model, HttpPostedFileBase file)
        {
            if (file != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] imageBytes = ms.GetBuffer();
                    model.article.image = imageBytes;
                }
            }
            manager.addNews(Models.NewsHelper.MappToCore(model));
            return RedirectToAction("Index");
        }

        public ActionResult MapReduce()
        {
            Models.MapReduceVm model = new Models.MapReduceVm();
            return View(model);
        }

        [HttpPost]
        public ActionResult MapReduce(int buttonID, string JSfunction)
        {
            var model = new Models.MapReduceVm();
            switch (buttonID)
            {
                case 1:
                    model.listArticlesComments = manager.mapReduce1(JSfunction);
                    break;
                case 2:
                    model.listAuthorWord = manager.mapReduce2(JSfunction);
                    break;
                case 3:
                    model.newsListTest = manager.mapReduceTest();
                    break;
                case 4:
                    model.newsListTest = manager.mapReduceTest();
                    break;
            }
            return View(model);
        }
    }
}
