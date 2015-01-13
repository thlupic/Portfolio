using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class NewsDB
    {
        public int newsID { get; set; }
        public string headline { get; set; }
        public string author { get; set; }
        public string text { get; set; }
        public byte[] image { get; set; }

        public List<Comment> comments { get; set; }

        public NewsDB()
        {
            comments = new List<Comment>();
        }
    }
}
