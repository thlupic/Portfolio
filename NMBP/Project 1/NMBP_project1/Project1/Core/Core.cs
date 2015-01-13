using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project1.Core
{
    public class Movie
    {
        public int ID { get; set; }
        public string name { get; set; }
        public double rank { get; set; }
    }

    public class searchResults
    {
        public string query { get; set; }
        public List<int> values { get; set; }

        public searchResults()
        {
            this.values = new List<int>();
        }
    }

    public class analyseResults
    {
        public List<searchResults> results { get; set; }
        public List<String> columns { get; set; }
        public string query { get; set; }

        public analyseResults()
        {
            this.columns = new List<string>();
            this.results = new List<searchResults>();
        }
    }

    public class serverSetup
    {
        public string server { get; set; }
        public string port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string DB { get; set; }

        public serverSetup setDefault()
        {
            serverSetup serverSetup = new serverSetup();
            serverSetup.server = "192.168.56.12";
            serverSetup.port = "5432";
            serverSetup.username = "postgres";
            serverSetup.password = "reverse";
            serverSetup.DB = "Movies";
            return serverSetup;
        }
    }
}
