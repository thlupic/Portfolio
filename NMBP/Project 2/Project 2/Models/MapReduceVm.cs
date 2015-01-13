using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core;

namespace Project_2.Models
{
    public class MapReduceVm
    {
        public string mapReduceJS { get; set; }
        public List<AuthorWord> listAuthorWord { get; set; }
        public List<ArticlesComments> listArticlesComments { get; set; }
        public List<News> newsListTest { get; set; }

        public MapReduceVm()
        {
            listAuthorWord = new List<AuthorWord>();
            listArticlesComments = new List<ArticlesComments>();
            newsListTest = new List<News>();
        }
    }
}