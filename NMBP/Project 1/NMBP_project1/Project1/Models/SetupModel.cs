using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project1.Models
{
    public class SetupModel
    {
        public string movieName { get; set; }
        //public string server { get; set; }
        //public string port { get; set; }
        //public string username { get; set; }
        //public string password { get; set; }
        //public string DB { get; set; }
        public string result { get; set; }
        public Core.serverSetup setupData { get; set; }

        public SetupModel()
        {
            this.result = "";
            this.setupData = new Core.serverSetup();
        }
    }
}
