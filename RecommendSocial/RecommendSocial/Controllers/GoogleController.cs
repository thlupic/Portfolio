using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Web.Script.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;

using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.IO;


namespace BootstrapMvcSample
{
    public class GoogleController : Controller
    {

        public  class  GoogleToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }


        }

        GoogleToken google_token = new GoogleToken();

        public string url_rt = "http://api.rottentomatoes.com/api/public/v1.0/lists/movies/box_office.json?limit=50&country=us&apikey=sn6wtjvtm67kh73j72vvhk57";
        
        public string google_client_id ="1049828568105-29k84e4g6q7poognbtkrb0245h59sacc.apps.googleusercontent.com";
        public string google_client_secret = "yZB76Q76tF6SF48g-wUDBMZi";
        public string api_key = "AIzaSyBiU_1m_o7YtpwwQIpuGQvHBLqcucdkVdI";

        public string url_auth = "https://accounts.google.com/o/oauth2/auth?client_id=1049828568105-29k84e4g6q7poognbtkrb0245h59sacc.apps.googleusercontent.com&redirect_uri=http://localhost:3665/google/signin&scope=https://www.googleapis.com/auth/youtube.readonly&response_type=code&access_type=offline";

        static string url_youtube = "https://www.googleapis.com/youtube/v3/playlists?part=snippet&mine=true&access_token=";

        public string json_data;


        public void databaseStoreMovie()
        {
            //spajanje na mongo
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("rotten");
            var collection = database.GetCollection("movies");

            //spoji se na rotten tomatoes, limit na 50 filmova
            var w = new WebClient();
            json_data = string.Empty;
            json_data = w.DownloadString(url_rt);
            json_data = json_data.Replace("\n", String.Empty);

            //spremi podatke u lokalnu bazu, za sljedeci labos spremi pametnije(samo raiting)
            BsonDocument doc = BsonDocument.Parse(json_data);
            collection.Insert(doc);
            server.Disconnect();
        }

        public void databaseStorePlaylists()
        {
            //spoji se na lokalnu bazu
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("youtube");
            var collection = database.GetCollection("playlists");

            //spoji se na YouTube access token posebno zaljepljen
            var w = new WebClient();
            json_data = string.Empty;
          //  url_youtube = url_youtube + google_token.access_token + "&key=AIzaSyBiU_1m_o7YtpwwQIpuGQvHBLqcucdkVdI";
            json_data = w.DownloadString(url_youtube);
       
            //spremi podatke u lokalnu bazu
            BsonDocument doc = BsonDocument.Parse(json_data);
            collection.Insert(doc);
            server.Disconnect();
        }

        public BsonDocument DBFindMovie ()
        {
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("rotten");
            var collection = database.GetCollection("movies").FindAll();
            BsonDocument doc = collection.First();
            return doc;  

        }

        public BsonDocument DBFindPlaylist()
        {
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("youtube");
            var collection = database.GetCollection("playlists").FindAll();
            BsonDocument doc = collection.First();
            return doc;  
        }

        public ActionResult SignIn()
        {
            return View();
        }



         [HttpGet]
        public ActionResult SignIn(string a)
        {
             //try blok kada google salje code  posalji request za access token
            
            try
            {
                a = Request["code"].ToString();

                var request = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/o/oauth2/token");

                var postData = "code="+ a;
                postData += "&client_id=1049828568105-29k84e4g6q7poognbtkrb0245h59sacc.apps.googleusercontent.com&";
                postData += "client_secret=yZB76Q76tF6SF48g-wUDBMZi&";
                postData += "redirect_uri=http://localhost:3665/google/signin&";
                    postData += "grant_type=authorization_code";
                var data = Encoding.UTF8.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                //ovaj dio s dohvacanjem json formata ljepse napravi 
                Stream s = response.GetResponseStream();                        

                TextReader textReader = new StreamReader(s, true);
                    
                //string jsonString = textReader.ReadToEnd();

                JavaScriptSerializer js = new JavaScriptSerializer();
                var objText = textReader.ReadToEnd();

                google_token = (GoogleToken)js.Deserialize(objText, typeof(GoogleToken));

                url_youtube = url_youtube + google_token.access_token + "&key=AIzaSyBiU_1m_o7YtpwwQIpuGQvHBLqcucdkVdI";

                databaseStorePlaylists();
                return View ("Client");

            }
            catch
            {
    
                ////////// Google auth    ///////////////
                var w = new WebClient();
                json_data = string.Empty;
                json_data = w.DownloadString(url_auth);
                return Content(json_data);
    
            }
        }


        public ActionResult RottenToematoes()
        {
            //spremi filmove 
            databaseStoreMovie();
            return Content(DBFindMovie().ToString());
        }

        public ActionResult StorePlaylists()
        {
            // spremi playliste u bazu
            databaseStorePlaylists();
            return Content(DBFindPlaylist().ToString());
        }

        public ActionResult Movies()
        {
            return Content(DBFindMovie().ToString());
        }

        public ActionResult Playlists()
        {
            //dohvati moje playliste 
            return Content(DBFindPlaylist().ToString());
        }           
       
        public ActionResult Client ()
        {

            return View();
        }




    }
}
