using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using RS.BLL;
using RS.Core;
using System.Diagnostics;
using Tweetinvi;
using WebService;
using Facebook;

namespace BootstrapMvcSample.Controllers
{
    public class HomeController : BootstrapBaseController
    {
        private static List<HomeInputModel> _models = ModelIntializer.CreateHomeInputModels();
        private static UserCore.userData user = new UserCore.userData();

        public ActionResult Index()
        {
            //var homeInputModels = _models;
            //var modelMovies = new RecommendSocial.Models.MovieVm();
            //var genresList = MovieDB.getAllGenres();
            //var JsonResult = MovieDB.getSearch(model.searchName);
            //modelMovies.searchName = model.searchName;
            //modelMovies.movies = MovieDB.MappToCore(JsonResult);
           // modelMovies.numberOfMovies = 0;

            //MovieDB.storeMovies(movieList);
            //MovieDB.storeGenres(genresList);
            return RedirectToAction("Login");
        }

        //  pocetna funkcija koja logira usera pomocu facebook accounta
        [HttpGet]
        public ActionResult Login()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "1538470139728022",
                client_secret = "958bde759598dbac3db3cf0eda526709",
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "user_likes,user_friends"
            });
            return Redirect(loginUrl.AbsoluteUri);
        }

        //pomocna funkcija koja se koristi kod logiranja s Facebook accountom
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "1538470139728022",
                client_secret = "958bde759598dbac3db3cf0eda526709",
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code,
                scope = "user_likes,user_friends"
            });

            // kada si dobio access token dohvati informacije o korisniku 
            var accessToken = result.access_token;
            fb.AccessToken = accessToken;

            //dohvati list svih korisnika iz baze
            List<UserCore.userData> users = RS.BLL.Facebook.getUsers();

            dynamic me = fb.Get("me?fields=first_name,last_name,id,likes,friends");
            //pronadi korisnika u bazi ili ako ne postoji spremi novog 
            user=RS.BLL.Facebook.CreateUser(me, users);

            return RedirectToAction("LogedIn");
        }

        //defaultni pogled, bez dohvata filmova
        public ActionResult LogedIn()
        {
            var model = new RecommendSocial.Models.MovieVm();
            return View(model);
        }

        //dohvat filmova ili punjenje baze nakon klika na gumb
        [HttpPost]
        public ActionResult LogedIn(string MoviesFetch)
        {
            var model = new RecommendSocial.Models.MovieVm();
            if (MoviesFetch == "fetch")
            {
                //kliknuto na prvi gumb
                var movies = MovieDB.getMoviesByGenres();
                model.movies = movies;
                model.numberOfMovies = movies.Count();
            }
            else if (MoviesFetch == "fetch2")
            {
                var movies = MovieDB.getMoviesByGenresNew();
                model.movies = movies;
                model.numberOfMovies = movies.Count();
            }
            else if (MoviesFetch == "FBtest")
            {
                var scrape = new WebService.IMDBScrape();
                scrape.getFacebookID("222222222");
            }
            else
            {
                //dohvat filmova iz baze po funkciji za preporuku
                var movies = RS.BLL.RecommendMovies.recommend(user);
                model.numberOfMovies = movies.Count;
                model.movies = movies;
            }
            return View(model);
        }

        public ActionResult MoviesIndex()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MoviesIndex(RecommendSocial.Models.MovieVm model)
        {
            //var JsonResult = MovieDB.getSearch(model.searchName);
            //model.searchName = model.searchName;
            //model.movies = MovieDB.MappToCore(JsonResult);
            return View("MoviesIndex", model);
        }
    }
}
