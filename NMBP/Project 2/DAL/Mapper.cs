using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace DAL
{
    public class Mapper
    {
        private static DBAccess database = new DBAccess();

        public List<News> GetAllNews(int n)
        {
            List<News> allNews = new List<News>();

            var newsDB = database.GetNews(n);

            foreach (var item in newsDB)
            {
                News newArticle = new News();
                newArticle.newsID = item.newsID;
                newArticle.headline = item.headline;
                newArticle.author = item.author;
                newArticle.text = item.text;
                newArticle.image = item.image;

                foreach (var comment in item.comments)
                {
                    Comment newComment = new Comment();
                    newComment.text = comment.text;
                    newComment.timestamp = comment.timestamp;

                    newArticle.comments.Add(newComment);
                }

                allNews.Add(newArticle);
            }
            return allNews;
        }

        public void AddNews(News article)
        {
            database.AddNews(article);
        }

        public void AddComment(int newsID, string comment)
        {
            database.AddComment(newsID, comment);
        }

        public List<News> GetAllNews()
        {
            List<News> allNews = new List<News>();

            var newsDB = database.GetAllNews();

            foreach (var item in newsDB)
            {
                News newArticle = new News();
                newArticle.newsID = item.newsID;
                newArticle.headline = item.headline;
                newArticle.author = item.author;
                newArticle.text = item.text;
                newArticle.image = item.image;

                foreach (var comment in item.comments)
                {
                    Comment newComment = new Comment();
                    newComment.text = comment.text;
                    newComment.timestamp = comment.timestamp;

                    newArticle.comments.Add(newComment);
                }

                allNews.Add(newArticle);
            }
            return allNews;
        }
    }
}
