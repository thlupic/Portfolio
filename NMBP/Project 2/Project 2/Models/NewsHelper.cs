using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core;
using BLL;

namespace Project_2.Models
{
    public class NewsHelper
    {
        private static Manager manager = new Manager();

        public static NewsFeedVm MappAllNewsToVm (List<News> allNews)
        {
            NewsFeedVm newsFeed = new NewsFeedVm();

            foreach (var item in allNews)
            {
                item.image64 = Convert.ToBase64String(item.image);
                newsFeed.newsList.Add(item);
            }

            return newsFeed;
        }

        public static News MappToCore(NewsVm newArticle)
        {
            News articleCore = new News();

            articleCore.author = newArticle.article.author;
            articleCore.headline = newArticle.article.headline;
            articleCore.image = newArticle.article.image;
            articleCore.text = newArticle.article.text;
            articleCore.published = DateTime.Now;

            return articleCore;
        }

    }
}
