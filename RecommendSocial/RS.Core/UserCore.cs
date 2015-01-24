using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RS.Core
{
    public class UserCore
    {
        public class userData
        {
            public ObjectId _id { get; set; }
            public int id { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string facebookID { get; set; }
            public List<LikeData> likes { get; set; }
            public List<UserCore.FriendData> friends { get; set; }

            public userData()
            {
                likes = new List<LikeData>();
                friends = new List<UserCore.FriendData>();
            }
        }
      

        public class LikeData
        {
            [JsonProperty("id")]
            public string likeID { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            
        }

        public class DBuserData
        {
            public int id { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string facebookID { get; set; }
            public List<LikeData> likes { get; set; }
            public List<UserCore.FriendData> friends { get; set; }
        }

        public class FriendData
        {
            public string friendID { get; set; }
            public List<LikeData> likes { get; set; }
             public FriendData()
            {
                likes = new List<LikeData>();
               
            }

        }
    }
}
