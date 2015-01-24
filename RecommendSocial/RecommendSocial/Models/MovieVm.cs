using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RS.Core;

namespace RecommendSocial.Models
{
    public class MovieVm
    {
        public string searchName { get; set; }
        public List<moviesCore.movieDBData> movies { get; set; }
        public int numberOfMovies { get; set; }

        public MovieVm()
        {
            this.movies = new List<moviesCore.movieDBData>();
        }
    }
}
