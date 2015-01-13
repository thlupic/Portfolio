using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using CorrugatedIron;
using CorrugatedIron.Comms;
using CorrugatedIron.Config;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;

namespace DAL
{
    public class DBAccess
    {
        private IRiakEndPoint cluster;// = RiakCluster.FromConfig("riakConfig");
        private IRiakClient client;

        public List<News> GetNews(int n)
        {
            List<News> news = new List<News>();

            var pingResult = client.Ping();

            var result1 = pingResult.ResultCode;
            var result2 = pingResult.ErrorMessage;

            Counter counter = new Counter();

            //client.DeleteBucket("articles");
            //client.DeleteBucket("counter");

            var result = client.Get("counter", "lastID");

            if (result.IsSuccess != true)
            {
                counter.lastID = 0;
                var o = new RiakObject("counter", "lastID", counter);
                var putResult = client.Put(o);
            }
            else
            {
                counter = result.Value.GetObject<Counter>();
            }

            var currentItem = counter.lastID;

            for (var i = 0; i < n; i++)
            {
                News currentNews = client.Get("articles", currentItem.ToString()).Value.GetObject<News>();
                news.Add(currentNews);
                currentItem = currentNews.previousNewsID;
                if (currentItem == 0) break;
            }

            return news;
        }

        public List<News> GetAllNews()
        {
            List<News> news = new List<News>();

            var pingResult = client.Ping();

            var result1 = pingResult.ResultCode;
            var result2 = pingResult.ErrorMessage;

            Counter counter = new Counter();

            //client.DeleteBucket("articles");
            //client.DeleteBucket("counter");

            var result = client.Get("counter", "lastID");

            if (result.IsSuccess != true)
            {
                counter.lastID = 0;
                var o = new RiakObject("counter", "lastID", counter);
                var putResult = client.Put(o);
            }
            else
            {
                counter = result.Value.GetObject<Counter>();
            }

            var currentItem = counter.lastID;

            while (true)
            {
                News currentNews = client.Get("articles", currentItem.ToString()).Value.GetObject<News>();
                news.Add(currentNews);
                currentItem = currentNews.previousNewsID;
                if (currentItem == 0) break;
            }

            return news;
        }

        public void AddNews(News article)
        {
            article.previousNewsID = client.Get("counter", "lastID").Value.GetObject<Counter>().lastID;
            article.newsID = article.previousNewsID + 1;
            var o = new RiakObject("articles", article.newsID.ToString(), article);
            var putResult = client.Put(o);
            Counter counter = new Counter();
            counter.lastID = article.newsID;
            //updating counter
            o = new RiakObject("counter", "lastID", counter);
            putResult = client.Put(o);
            var result = putResult;
        }

        public void AddComment(int newsID, string comment)
        {
            //client.DeleteBucket("comments");
            News article = client.Get("articles", newsID.ToString()).Value.GetObject<News>();
            Comment commentCore = new Comment();
            commentCore.text = comment;
            commentCore.timestamp = DateTime.Now;
            article.comments.Add(commentCore);
            var o = new RiakObject("articles", newsID.ToString(), article);
            var result = client.Put(o);
            var putResult = result;
        }

        public DBAccess()
        {
            cluster = RiakCluster.FromConfig("riakConfig");
            client = cluster.CreateClient();
        }
    }
}
