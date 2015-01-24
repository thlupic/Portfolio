using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RS.Core;

namespace Models.Models
{

    public class UserProfile
    {
        public List<UserCore.userData> users { get; set; }

        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string facebookID { get; set; }
        public List<UserCore.LikeData> likes { get; set; }
        public List<UserCore> friends { get; set; }
    }
}