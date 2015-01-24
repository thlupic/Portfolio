using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;

namespace WebService
{
    public class IMDBScrape
    {
        //getting FB page ID
        public string getFacebookID(string IMDBID)
        {
            try
            {
                string URI = String.Format("http://www.imdb.com/title/tt{0}", "0816692");

                string html = new WebClient().DownloadString(URI);

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                HtmlNode docNode = htmlDoc.DocumentNode;
                HtmlNodeCollection nodes = docNode.SelectNodes("//div[id=\"titleDetails\"]");

                var singleNode = docNode.SelectSingleNode("//*[@id=\"titleDetails\"]");                

                var divFacebook = singleNode.ChildNodes[5].ChildNodes[3];

                var FBLink = String.Format("http://www.imdb.com{0}", divFacebook.Attributes[0].Value);

                var scrapper = new HTMLScrapping.WebSession();
                var FBHTML = scrapper.RequestPage(FBLink);

                var FBHTML1 = scrapper.RequestPage(FBLink);

                var pageIDString = FBHTML1.Html.Substring(FBHTML1.Html.IndexOf("pageID"), 50).Split(':')[1].Split(',')[0];

                return String.Format("https://www.facebook.com/{0}", pageIDString);
            }
            catch (Exception e)
            {
                var ex = e;
            }

            return null;
        }
    }
}
