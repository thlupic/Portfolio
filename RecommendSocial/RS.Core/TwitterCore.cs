using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tweetinvi;
using Tweetinvi.Core.Interfaces.Credentials;

namespace RS.Core
{
    public class TwitterCore
    {
        public class TwitterAuth
        {
            public string AuthKey { get; set; }
            public string AuthSecret { get; set; }
            public string captcha { get; set; }
            public string url { get; set; }
            public string oAuthConsumerKey = "CpGmHGNjVIIfxJ3rEB5nX4zXH";
            public string oAuthConsumerSecret = "tdc55jotGTiNbFkZRymLLltqQpCZDv0BBHY4zq8xOeLsY1UWbV";
        }

        public class Tokens
        {
            public string AccessToken { get; set; }
            public string AccessTokenSecret { get; set; }
        }

        public class User
        {
            public ObjectId _id { get; set; }
            public long ID { get; set; }
            public string AccessToken { get; set; }
            public string AccessTokenSecret { get; set; }
        }

    }
}
