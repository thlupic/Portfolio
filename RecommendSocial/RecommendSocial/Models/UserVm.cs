using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RecommendSocial.Models
{
    public class UserVm 
    {
        public string username { get; set; }
        public string password { get; set; }
        public long twitterID { get; set; }
    }
}
