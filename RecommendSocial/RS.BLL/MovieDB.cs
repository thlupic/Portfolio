using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RS.Core;
using WebService;
using MongoDB.Bson;
using RS.DAL;

namespace RS.BLL
{
    public class MovieDB
    {
        public static string apiKey = "14526d01d666a48a486e579c77ba4d74";
        public static string RTapiKey = "9xxvxysqzw9xqe9nw8p54tdc";
        private static Methods webMethods = new Methods();
        private static Serialization serializer = new Serialization();
        private static IMDBScrape scrapper = new IMDBScrape();

        //dohvati sve zanrove sa TMDB
        public static moviesCore.movieGenres getAllGenres()
        {
            string URI = String.Format("http://api.themoviedb.org/3/genre/movie/list?api_key={0}", apiKey);

            return JsonConvert.DeserializeObject<moviesCore.movieGenres>(webMethods.GET(URI));
        }

        //dohvaca sve filmove razvrstane po zanrovima i pridjeljuje im vrijdnosti iz TMDB, IMDB i RottenTomatoes i sprema u bazu
        public static List<moviesCore.movieDBData> getMoviesByGenres()
        {
            HashSet<moviesCore.movieDBData> movieData = new HashSet<moviesCore.movieDBData>();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string URI = "";

            var genres = getAllGenres();

            int totalResults = 0;

            foreach (var item in genres.genreList)
            {
                if (totalResults > 9900) break;
                URI = String.Format("http://api.themoviedb.org/3/genre/{0}/movies?api_key={1}", item.genreID, apiKey);

                var movies = JsonConvert.DeserializeObject<moviesCore.moviesData>(webMethods.GET(URI));

                for (var i = 1; i <= (movies.totalPages); i++)
                {
                        URI = String.Format("http://api.themoviedb.org/3/genre/{0}/movies?api_key={1}&page={2}", item.genreID, apiKey, i);

                        var movies1 = JsonConvert.DeserializeObject<moviesCore.moviesData>(webMethods.GET(URI));

                        foreach (var movie in movies1.movies)
                        {
                            try
                            {
                                URI = String.Format("http://api.themoviedb.org/3/movie/{0}?api_key={1}", movie.id, apiKey);
                                var genresMovie = JsonConvert.DeserializeObject<moviesCore.movieGenres>(webMethods.GET(URI));
                                var name = movie.title;
                                var nameParts = name.Split(' ');
                                string nameSearch = "";
                                foreach (var namepart in nameParts)
                                {
                                    nameSearch += String.Format("+{0}", namepart);
                                }
                                nameSearch = nameSearch.Split(':')[0]; //ima problema s pronalazenjem nekih filmova zbog krivih prijevoda iza :
                                nameSearch = nameSearch.Substring(1, nameSearch.Length - 1);
                                var releaseDate = movie.date;
                                string year = "";
                                if (releaseDate != "") { year = Convert.ToDateTime(releaseDate).Year.ToString(); }
                                URI = String.Format("http://www.omdbapi.com/?t={0}&y={1}&plot=short&r=json", nameSearch.ToLower(), year);
                                //allMovies.Add(movie);

                                var OMDBmovie = JsonConvert.DeserializeObject<moviesCore.movieDataOMDB>(webMethods.GET(URI));
                                //OMDBMovies.Add(OMDBmovie);
                                var imdbID = OMDBmovie.imdbID;
                                if (imdbID != null)
                                {
                                    imdbID = imdbID.Substring(2, imdbID.Length - 2);

                                    string facebookLink = scrapper.getFacebookID(imdbID);
                                    try
                                    {
                                        URI = String.Format("http://api.rottentomatoes.com/api/public/v1.0/movie_alias.json?apikey={0}&type=imdb&id={1}", RTapiKey, imdbID);
                                    }
                                    catch
                                    {
                                        if (RTapiKey == "9xxvxysqzw9xqe9nw8p54tdc") RTapiKey = "sn6wtjvtm67kh73j72vvhk57"; else RTapiKey = "9xxvxysqzw9xqe9nw8p54tdc";
                                        URI = String.Format("http://api.rottentomatoes.com/api/public/v1.0/movie_alias.json?apikey={0}&type=imdb&id={1}", RTapiKey, imdbID);
                                    }
                                    var RTdata = webMethods.GET(URI);
                                    var RTcore = JsonConvert.DeserializeObject<moviesCore.movieDataRT>(RTdata);
                                    //RTMovies.Add(RTcore);

                                    // dodavanje podataka u klasu za filmove
                                    var singleMovieData = new moviesCore.movieDBData();
                                    singleMovieData.IMDBID = imdbID;
                                    if (OMDBmovie.imdbRating != "N/A")
                                    {
                                        singleMovieData.imdbScore = Math.Round(Convert.ToDouble(OMDBmovie.imdbRating)/10, 2, MidpointRounding.AwayFromZero);
                                    }
                                    singleMovieData.TMDBID = movie.id;
                                    singleMovieData.title = movie.title;
                                    if (year != "")
                                    {
                                        singleMovieData.year = Convert.ToInt32(year);
                                    }
                                    singleMovieData.RTID = RTcore.id.ToString();
                                    singleMovieData.cast = RTcore.cast;
                                    singleMovieData.genres = genresMovie;
                                    singleMovieData.director = OMDBmovie.director;
                                    singleMovieData.genres = genresMovie;
                                    singleMovieData.facebookLink = facebookLink;
                                    if (singleMovieData.cast.Count > 0)
                                    {
                                        var newValue = movieData.Add(singleMovieData);
                                        if (newValue) RS.DAL.DataStorage.storeMovie(singleMovieData);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                var ex = e;
                            }
                        }
                    }
            }

            totalResults += 0;

            List<moviesCore.movieDBData> movieDataList = new List<moviesCore.movieDBData>();
            foreach (var item in movieData) movieDataList.Add(item);
            return movieDataList;
        }

        public static List<moviesCore.movieDBData> getMoviesByGenresNew()
        {
            HashSet<moviesCore.movieDBData> movieData = new HashSet<moviesCore.movieDBData>();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string URI = "";

            var genres = getAllGenres();

            int totalResults = 0;

            foreach (var item in genres.genreList)
            {
                if (totalResults > 9900) break;
                URI = String.Format("http://api.themoviedb.org/3/genre/{0}/movies?api_key={1}", item.genreID, apiKey);

                var movies = JsonConvert.DeserializeObject<moviesCore.moviesData>(webMethods.GET(URI));

                for (var i = 1; i <= (movies.totalPages); i++)
                {
                    URI = String.Format("http://api.themoviedb.org/3/genre/{0}/movies?api_key={1}&page={2}", item.genreID, apiKey, i);

                    var movies1 = JsonConvert.DeserializeObject<moviesCore.moviesData>(webMethods.GET(URI));

                    foreach (var movie in movies1.movies)
                    {
                        try
                        {
                            URI = String.Format("http://api.themoviedb.org/3/movie/{0}?api_key={1}", movie.id, apiKey);
                            var genresMovie = JsonConvert.DeserializeObject<moviesCore.movieGenres>(webMethods.GET(URI));
                            var name = movie.title;
                            var nameParts = name.Split(' ');
                            string nameSearch = "";
                            foreach (var namepart in nameParts)
                            {
                                nameSearch += String.Format("+{0}", namepart);
                            }
                            nameSearch = nameSearch.Split(':')[0]; //ima problema s pronalazenjem nekih filmova zbog krivih prijevoda iza :
                            nameSearch = nameSearch.Substring(1, nameSearch.Length - 1);
                            var releaseDate = movie.date;
                            string year = "";
                            if (releaseDate != "") { year = Convert.ToDateTime(releaseDate).Year.ToString(); }
                            URI = String.Format("http://www.omdbapi.com/?t={0}&y={1}&plot=short&r=json", nameSearch.ToLower(), year);
                            //allMovies.Add(movie);

                            var OMDBmovie = JsonConvert.DeserializeObject<moviesCore.movieDataOMDB>(webMethods.GET(URI));
                            //OMDBMovies.Add(OMDBmovie);
                            var imdbID = OMDBmovie.imdbID;
                            if (imdbID != null)
                            {
                                imdbID = imdbID.Substring(2, imdbID.Length - 2);

                                string facebookLink = scrapper.getFacebookID(imdbID);
                                try
                                {
                                    URI = String.Format("http://api.rottentomatoes.com/api/public/v1.0/movie_alias.json?apikey={0}&type=imdb&id={1}", RTapiKey, imdbID);
                                }
                                catch
                                {
                                    if (RTapiKey == "9xxvxysqzw9xqe9nw8p54tdc") RTapiKey = "sn6wtjvtm67kh73j72vvhk57"; else RTapiKey = "9xxvxysqzw9xqe9nw8p54tdc";
                                    URI = String.Format("http://api.rottentomatoes.com/api/public/v1.0/movie_alias.json?apikey={0}&type=imdb&id={1}", RTapiKey, imdbID);
                                }
                                var RTdata = webMethods.GET(URI);
                                var RTcore = JsonConvert.DeserializeObject<moviesCore.movieDataRT>(RTdata);
                                //RTMovies.Add(RTcore);

                                // dodavanje podataka u klasu za filmove
                                var singleMovieData = new moviesCore.movieDBData();
                                singleMovieData.IMDBID = imdbID;
                                if (OMDBmovie.imdbRating != "N/A")
                                {
                                    singleMovieData.imdbScore = Math.Round(Convert.ToDouble(OMDBmovie.imdbRating) / 10, 2, MidpointRounding.AwayFromZero);
                                }
                                singleMovieData.TMDBID = movie.id;
                                singleMovieData.title = movie.title;
                                if (year != "")
                                {
                                    singleMovieData.year = Convert.ToInt32(year);
                                }
                                singleMovieData.RTID = RTcore.id.ToString();
                                singleMovieData.cast = RTcore.cast;
                                singleMovieData.genres = genresMovie;
                                singleMovieData.director = OMDBmovie.director;
                                singleMovieData.genres = genresMovie;
                                singleMovieData.facebookLink = facebookLink;
                                if (singleMovieData.cast.Count > 0)
                                {
                                    var newValue = movieData.Add(singleMovieData);
                                    if (newValue) RS.DAL.DataStorage.storeMovieNew(singleMovieData);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            var ex = e;
                        }
                    }
                }
            }

            totalResults += 0;

            List<moviesCore.movieDBData> movieDataList = new List<moviesCore.movieDBData>();
            foreach (var item in movieData) movieDataList.Add(item);
            return movieDataList;
        }

        //sprema filmove, sve odjednom
        public static void storeMovies(List<moviesCore.movieDBData> movies)
        {
            foreach (var item in movies)
            {
                RS.DAL.DataStorage.storeMovie(item);
            }
        }

        //sprema zanrove u bazu, sve odjednom
        public static void storeGenres(moviesCore.movieGenres genres)
        {
            foreach(var item in genres.genreList)
            {
                RS.DAL.DataStorage.storeGenre(item);
            }
        }

        public static object DeserializeFromStream(Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize(jsonTextReader);
            }
        }

        // s ovim se pretvara iz database objecta u object nad kojim radimo u aplikaciji
        public static List<moviesCore.movieDBData> MappToCore(List<moviesCore.movieDB> JSONMovies)
        {
            List<moviesCore.movieDBData> moviesData = new List<moviesCore.movieDBData>();
            foreach (var item in JSONMovies) moviesData.Add(item.data);
            return moviesData;
        }

        //dohvat svih filmova
        public static List<moviesCore.movieDB> getAllMovies()
        {
            List<moviesCore.movieDB> moviesDB = new List<moviesCore.movieDB>();

            moviesDB = DataStorage.getMovies();

            return moviesDB;
        }
    }
}