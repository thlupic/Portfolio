using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RS.Core
{

    public class moviesCore    
    {
        [BsonIgnoreExtraElements]
        public class movieDB
        {
            public ObjectId _id { get; set; }

            public movieDBData data { get; set; }

            //[JsonProperty("IMDBID")]
            //public string IMDBID { get; set; }

            //[JsonProperty("TMDBID")]
            //public string TMDBID { get; set; }

            //[JsonProperty("RTID")]
            //public string RTID { get; set; }

            //[JsonProperty("title")]
            //public string title { get; set; }

            //[JsonProperty("year")]
            //public int year { get; set; }

            //[JsonProperty("cast")]
            //public List<actorRT> cast { get; set; }

            //[JsonProperty("genres")]
            //public movieGenres genres { get; set; }

            //[JsonProperty("imdbScore")]
            //public double imdbScore { get; set; }

            //[JsonProperty("director")]
            //public string director { get; set; }

            //[JsonProperty("facebookLink")]
            //public string facebookLink { get; set; }

            //public movieDB()
            //{
            //    cast = new List<actorRT>();
            //    genres = new movieGenres();
            //}
        }

        public class movieDBDB : movieDBData
        {
            public ObjectId _id { get; set; }
        }

        public class movieDBData
        {
            //public ObjectId _id { get; set; }
            [JsonProperty("IMDBID")]
            public string IMDBID { get; set; }

            [JsonProperty("TMDBID")]
            public string TMDBID { get; set; }

            [JsonProperty("RTID")]
            public string RTID { get; set; }

            [JsonProperty("title")]
            public string title { get; set; }

            [JsonProperty("year")]
            public int year { get; set; }

            [JsonProperty("cast")]
            public List<actorRT> cast { get; set; }

            [JsonProperty("genres")]
            public movieGenres genres { get; set; }

            [JsonProperty("imdbScore")]
            public double imdbScore { get; set; }

            [JsonProperty("director")]
            public string director { get; set; }

            [JsonProperty("facebookLink")]
            public string facebookLink { get; set; }

            public movieDBData()
            {
                cast = new List<actorRT>();
                genres = new movieGenres();
            }
        }

        public class movieDataTMDB
        {
            [JsonProperty("id")]
            public string id { get; set; }

            [JsonProperty("original_title")]
            public string originalTitle { get; set; }

            [JsonProperty("release_date")]
            public string date { get; set; }

            [JsonProperty("title")]
            public string title { get; set; }

            [JsonProperty("genres")]
            public List<moviesGenreTMDB> genres { get; set; }

            public movieDataTMDB()
            {
                genres = new List<moviesGenreTMDB>();
            }
        }

        public class moviesData
        {
            [JsonProperty("page")]
            public int Page { get; set; }

            [JsonProperty("results")]
            public List<movieDataTMDB> movies { get; set; }

            [JsonProperty("total_pages")]
            public int totalPages { get; set; }

            [JsonProperty("total_results")]
            public int totalResults { get; set; }
        }

        public class movieGenres
        {
            [JsonProperty("genres")]
            public List<moviesGenreTMDB> genreList { get; set; }

            public movieGenres()
            {
                genreList = new List<moviesGenreTMDB>();
            }
        }

        public class moviesGenreTMDB
        {
            [JsonProperty("id")]
            public int genreID { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }
        }

        public class movieDataOMDB
        {
            [JsonProperty("Title")]
            public string title { get; set; }

            [JsonProperty("Year")]
            public string year { get; set; }

            [JsonProperty("Rated")]
            public string rated { get; set; }

            [JsonProperty("Released")]
            public string released { get; set; }

            [JsonProperty("Runtime")]
            public string runtime { get; set; }

            [JsonProperty("Genre")]
            public string genre { get; set; }

            [JsonProperty("Director")]
            public string director { get; set; }

            [JsonProperty("Writer")]
            public string writer { get; set; }

            [JsonProperty("Actors")]
            public string actors { get; set; }

            [JsonProperty("Plot")]
            public string plot { get; set; }

            [JsonProperty("Language")]
            public string language { get; set; }

            [JsonProperty("Country")]
            public string country { get; set; }

            [JsonProperty("Awards")]
            public string awards { get; set; }

            [JsonProperty("Poster")]
            public string poster { get; set; }

            [JsonProperty("Metascore")]
            public string metascore { get; set; }

            [JsonProperty("imdbRating")]
            public string imdbRating { get; set; }

            [JsonProperty("imdbVotes")]
            public string imdbVotes { get; set; }

            [JsonProperty("imdbID")]
            public string imdbID { get; set; }

            [JsonProperty("Type")]
            public string type { get; set; }

            [JsonProperty("Response")]
            public string response { get; set; }
        }

        public class movieDataRT
        {
            [JsonProperty("id")]
            public int id { get; set; }

            [JsonProperty("abridged_cast")]
            public List<actorRT> cast { get; set; }

            public movieDataRT()
            {
                cast = new List<actorRT>();
            }
        }

        public class castRT
        {
            [JsonProperty("cast")]
            public List<actorRT> cast { get; set; }

            public castRT()
            {
                cast = new List<actorRT>();
            }
        }

        public class actorRT
        {
            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("characters")]
            public List<string> characters { get; set; }

            public actorRT()
            {
                characters = new List<string>();
            }
        }
    }
}
