using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using DAL;

namespace BLL
{    
    public class Manager
    {
        private static Mapper mapper = new Mapper();
        private static MapReduce mapReduce = new MapReduce();
        public List<News> getAllNews(int n)
        {
            return mapper.GetAllNews(n);
        }

        public void addNews(News newArticle)
        {
            mapper.AddNews(newArticle);
        }

        public void addComment(int newsID, string commentText)
        {
            mapper.AddComment(newsID, commentText);
        }

        public List<ArticlesComments> mapReduce1(string JS)
        {
            return mapReduce.mapReduce1(JS);
        }

        public List<AuthorWord> mapReduce2(string JS)
        {
            return mapReduce.mapReduce2(JS);
        }

        public List<News> mapReduceTest()
        {
            return mapper.GetAllNews();
        }
    }
}
