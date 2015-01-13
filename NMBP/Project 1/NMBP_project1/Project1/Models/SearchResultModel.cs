using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project1.Models
{
    public class SearchResultModel
    {
        public string SearchValue { get; set; }
        public List<String> SearchResult { get; set; }
        public List<double> rank { get; set; }
        public List<String> Columns { get; set; }

        public string SearchValueText { get; set; }
        public bool QueryAndValue { get; set; }
        public string SearchType { get; set; }
        public string SQLQuery { get; set; }

        public SearchResultModel()
        {
            this.SearchResult = new List<string>();
            this.rank = new List<double>();
            this.Columns = new List<string>();
        }
    }
}
