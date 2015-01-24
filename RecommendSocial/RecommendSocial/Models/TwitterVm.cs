using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace RecommendSocial.Models
{
    public class TwitterVm
    {
        public string captcha { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string profileImageLink { get; set; }
        public bool isAuth { get; set; }
        public string searchName { get; set; }
        public long twitterID { get; set; }
    }
}
