using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core;

namespace Project_2.Models
{
    public class NewsVm
    {
        public Core.News article { get; set; }

        public NewsVm()
        {
            article = new Core.News();
        }
    }
}
