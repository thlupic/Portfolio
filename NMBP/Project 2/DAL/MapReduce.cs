using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CorrugatedIron;
using Core;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using System.Reflection;

namespace DAL
{
    public class MapReduce
    {
        private static DBAccess db = new DBAccess();
        private IRiakEndPoint cluster;
        private IRiakClient client;

        public List<ArticlesComments> mapReduce1(string JS)
        {
            List<ArticlesComments> list = new List<ArticlesComments>();

            var newsNumber = db.GetAllNews().Count;

            var inputs = new RiakBucketKeyInput();

            for (var i = 1; i <= newsNumber; i++)
            {
                inputs.Add("articles", i.ToString());
            }

            var JSmap = "function (obj) {var jsx=JSON.parse(obj.values[0].data);var value1=jsx.comments;var count=[];var result = 0;for (var prop in value1) { if (value1.hasOwnProperty(prop)) { result++; } };count.push({key:obj.key,count:result,headline:jsx.headline});return count;}";

            var JSreduce = "function(obj){return obj.sort(function(a, b) { return JSON.parse(b.count) - JSON.parse(a.count) })}";

            var query = new RiakMapReduceQuery().Inputs("articles").MapJs(x => x.Source(JSmap)).ReduceJs(x=>x.Source(JSreduce).Keep(true));

            var result = client.MapReduce(query);

            if (result.IsSuccess)
            {
                foreach (var phase in result.Value.PhaseResults)
                {
                    // this contains the phase index
                    var phaseNume = phase.Phase;

                    // get access to the value in various ways
                    var sumResult = phase.GetObjects();

                    var serializer = new JavaScriptSerializer();
                    if (sumResult.Count() > 0)
                    {
                        foreach (var item in sumResult)
                        {
                            var jsd = serializer.DeserializeObject(Convert.ToString(item));
                            foreach (var items in jsd)
                            {
                                List<KeyValuePair<String, object>> listKVP = new List<KeyValuePair<string, object>>();
                                foreach (KeyValuePair<string, object> kvp in items) listKVP.Add(kvp);
                                ArticlesComments articleComment = new ArticlesComments();
                                articleComment.key = listKVP.ElementAt(0).Value.ToString();
                                articleComment.commentNumber = Convert.ToInt32(listKVP.ElementAt(1).Value);
                                articleComment.articleName = listKVP.ElementAt(2).Value.ToString();
                                list.Add(articleComment);
                            }
                        }
                    }
                }
            }


            return list;
        }

        public List<AuthorWord> mapReduce2(string JS)
        {
            List<AuthorWord> list = new List<AuthorWord>();
            #region oldMRcode
            /*
                    private class keyValue
        {
            public string key { get; set; }
        }

        private class wordList
        {
            public string word { get; set; }
            public int count { get; set; }
        }

            var JSmap11 = "function (obj) {var jsx=JSON.parse(obj.values[0].data);var value1=jsx.author;var value=[];value.push({author:value1,key:obj.key});return value;}";

            var JSred11 = "function(values) {var result=[];var authors=[];for (var entry in values){var key=values[entry].key;var author=values[entry].author;if (authors.indexOf(author)==-1) authors.push(author);}for (var unique in authors){var authorName = authors[unique];var keys=[];for (var entry1 in values){var author = values[entry1].author;var key=values[entry1].key;if (author==authorName) keys.push({key:key});}result.push({author:authorName,keys:keys});}return result}";

            var queryAuthors = new RiakMapReduceQuery().Inputs("articles").MapJs(x=>x.Source(JSmap11)).ReduceJs(x=>x.Source(JSred11).Keep(true));

            var resultAuthors = client.MapReduce(queryAuthors);

            var authorsKeys = new List<AuthorKeys>();

            if (resultAuthors.IsSuccess)
            {
                foreach (var phase in resultAuthors.Value.PhaseResults)
                {
                    // this contains the phase index
                    var phaseNume = phase.Phase;

                    // get access to the value in various ways
                    var sumResult = phase.GetObjects();

                    var serializer = new JavaScriptSerializer();
                    if (sumResult.Count() > 0)
                    {
                        foreach (var item in sumResult)
                        {
                            var jsd = serializer.DeserializeObject(Convert.ToString(item));
                            foreach (var items in jsd)
                            {                                
                                List<KeyValuePair<String, object>> listKVP = new List<KeyValuePair<string, object>>();
                                List<KeyValuePair<String, object>> listKVP1 = new List<KeyValuePair<string, object>>();
                                foreach (KeyValuePair<string, object> kvp in items) listKVP.Add(kvp);
                                AuthorKeys authorKeys = new AuthorKeys();
                                authorKeys.authorName = listKVP.ElementAt(0).Value.ToString();

                                string x = serializer.Serialize(listKVP.ElementAt(1).Value);
                                //var dx = serializer.DeserializeObject(Convert.ToString(listKVP.ElementAt(1).Value));

                                List<keyValue> keyValueList = (List<keyValue>)Newtonsoft.Json.JsonConvert.DeserializeObject(x,typeof(List<keyValue>));

                                foreach (var keyItem in keyValueList)
                                {
                                    authorKeys.keysList.Add(keyItem.key);
                                }
                                
                                authorsKeys.Add(authorKeys);
                            }
                        }
                    }
                }
            }

            /*
            foreach (var item in authorsKeys)
            {
                List<wordList> wordsList = new List<wordList>();
                var inputs = new RiakBucketKeyInput();

                //dodamo sve clanke iz bucketa articles odredjenog autora u input
                for (var i = 0; i < item.keysList.Count; i++)
                {
                    inputs.Add("articles", item.keysList[i]);
                }

                //var JSmap1 = @"function(v) {var vx=JSON.parse(v.values[0].data);var m = vx.text.toLowerCase().match(/\w* /g);var r = [];for(var i in m) {if(m[i] != '') {var o = {};o[m[i]] = 1;r.push(o);}}return r;}";

                var JSred1 = @"function(v) {var r = {};for(var i in v) {for(var w in v[i]) {if(w in r) r[w] += v[i][w];else r[w] = v[i][w];}}return [r];}";

                var queryWords = new RiakMapReduceQuery().Inputs(inputs).MapJs(x => x.Source(JSmap1)).ReduceJs(x => x.Source(JSred1).Keep(true));

                var resultWords = client.MapReduce(queryWords);

                if (resultWords.IsSuccess)
                {
                    foreach (var phase in resultWords.Value.PhaseResults)
                    {
                        var phaseNume = phase.Phase;

                        // get access to the value in various ways
                        var sumResult = phase.GetObjects();
                        var serializer = new JavaScriptSerializer();
                        foreach (var itemResult in sumResult)
                        {
                            var jsd = serializer.DeserializeObject(Convert.ToString(itemResult));
                            foreach (var jsdItems in jsd)
                            {
                                List<KeyValuePair<String, object>> listKVP = new List<KeyValuePair<string, object>>();
                                foreach (KeyValuePair<string, object> kvp in jsdItems) listKVP.Add(kvp);
                                foreach (var itemx in listKVP)
                                {
                                    wordList word = new wordList();
                                    word.count = (int)itemx.Value;
                                    word.word = itemx.Key;
                                    wordsList.Add(word);
                                }
                            }
                        }
                    }
                }

                wordsList = wordsList.OrderByDescending(x => x.count).ToList();

                AuthorWord authorWord = new AuthorWord();
                authorWord.authorName = item.authorName;
                for (var i = 0; i < 10; i++)
                {
                    var word = new Word();
                    word.word = wordsList.ElementAt(i).word;
                    word.count = wordsList.ElementAt(i).count;
                    authorWord.mostUsedWords.Add(word);
                }
                list.Add(authorWord);
            }*/
            #endregion

            /* new code */

            var JSmap = "function (obj) {var jsx=JSON.parse(obj.values[0].data);var value1=jsx.author;var value2=jsx.text;var value=[];value.push({author:value1,text:value2});return value;}";

            var JSred = "function(values) {var result=[];var authors=[];for (var entry in values){var key=values[entry].text;var author=values[entry].author;if (authors.indexOf(author)==-1) authors.push(author);}for (var unique in authors){var authorName = authors[unique];var keys=[];for (var entry1 in values){var author = values[entry1].author;var key=values[entry1].text;if (author==authorName) keys.push(key);}var text=new String(); for(var item in keys){var text1=keys[item];text+=text1;};result.push({author:authorName,texts:text});}return result;}";

            var JSred2 = "function(values) {  var resultG = [];for (var value in values) {var texts = values[value].texts;var words=texts.toLowerCase().split(/[,. ]/);var counts=[];for(var word1 in words) {if (words[word1] != '') {var count = {};count[words[word1]] = 1;counts.push(count);}} var author = values[value].author;var result = {}; for(var value in counts) {for(var word in counts[value]){if (word in result) result[word] += counts[value][word]; else result[word] = counts[value][word];}}var resultArray = [];for(var item in result){resultArray.push({word:item,count:result[item]});}resultArray.sort(function(a, b) {return JSON.parse(b.count) - JSON.parse(a.count)});var finalArray=resultArray.slice(0,10);resultG.push({author:author,words:finalArray});}return resultG;}";

            var queryAuthorsWords = new RiakMapReduceQuery().Inputs("articles").MapJs(x => x.Source(JSmap)).ReduceJs(x => x.Source(JSred).Keep(true)).ReduceJs(x=>x.Source(JSred2).Keep(true));

            var result = client.MapReduce(queryAuthorsWords);

            if (result.IsSuccess)
            {
                foreach (var phase in result.Value.PhaseResults)
                {
                    var phaseNume = phase.Phase;

                    var sumResult = phase.GetObjects();

                    var serializer = new JavaScriptSerializer();

                    if (phaseNume == result.Value.PhaseResults.Count() - 1)
                    {
                        foreach (var itemResult in sumResult)
                        {
                            var jsd = serializer.DeserializeObject(Convert.ToString(itemResult));
                            foreach (var jsdItem in jsd)
                            {
                                List<KeyValuePair<String, object>> listKVP = new List<KeyValuePair<string, object>>();
                                foreach (KeyValuePair<string, object> kvp in jsdItem) listKVP.Add(kvp);
                                var authorWord = new AuthorWord();
                                authorWord.authorName = listKVP.ElementAt(0).Value.ToString();
                                var words = listKVP.ElementAt(1).Value;
                                var wordsString = serializer.Serialize(words);
                                List<Word> keyValueList = (List<Word>)Newtonsoft.Json.JsonConvert.DeserializeObject(wordsString, typeof(List<Word>));

                                foreach (var item in keyValueList) authorWord.mostUsedWords.Add(item);
                                list.Add(authorWord);
                            }
                        }
                    }
                }
            }

            return list;
        }

        public MapReduce()
        {
            cluster = RiakCluster.FromConfig("riakConfig");
            client = cluster.CreateClient();
        }
    }
}
