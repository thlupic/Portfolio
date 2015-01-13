using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core;

namespace Project_2.Models
{
    public class NewsFeedVm 
    {
        public List<Core.News> newsList { get; set; }

        public NewsFeedVm()
        {
            newsList = new List<Core.News>();
        }
    }
}
