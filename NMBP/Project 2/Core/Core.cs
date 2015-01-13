using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class News
    {
        public string headline { get; set; }
        public string author { get; set; }
        public string text { get; set; }
        public byte[] image { get; set; }
        public string image64 { get; set; } 
        public DateTime published { get; set; }
        public int newsID { get; set; }
        public int previousNewsID { get; set; }

        public List<Comment> comments { get; set; }

        public News()
        {
            comments = new List<Comment>();
        }
    }

    public class Comment
    {
        public DateTime timestamp { get; set; }
        public string text { get; set; }
    }

    public class Counter
    {
        public int lastID { get; set; }
    }

    public class AuthorWord
    {
        public string authorName { get; set; }
        public List<Word> mostUsedWords {get;set;}

        public AuthorWord()
        {
            mostUsedWords = new List<Word>();
        }
    }

    public class Word
    {
        public string word {get;set;}
        public int count {get;set;}
    }


    public class AuthorKeys
    {
        public string authorName { get; set; }
        public List<string> keysList { get; set; }

        public AuthorKeys()
        {
            keysList = new List<string>();
        }
    }

    public class ArticlesComments
    {
        public string key { get; set; }
        public string articleName { get; set; }
        public int commentNumber { get; set; }
    }
    
}
