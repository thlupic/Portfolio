using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project1.BLL
{
    public class Manager
    {
        private DAL.DBAccess database { get; set; }

        #region buildGeneralQuery
        public List<Core.Movie> getAllMovies()
        {
            return database.getAllMovies();
        }

        public string buildFullQuery(List<String> queryElements, bool isAndOperator, string SearchType)
        {
            string query = "";
            string querySelect = buildQuerySelect(queryElements, isAndOperator);
            string queryWhere = "";
            switch(SearchType)
            {
                case "exact":
                    queryWhere = buildQueryExact2(queryElements, isAndOperator);
                    break;
                case "dictionary":
                    queryWhere = buildQueryDictionary(queryElements, isAndOperator);
                    break;
                case "fuzzy":
                    queryWhere = buildQueryFuzzy(queryElements, isAndOperator);
                    break;
                default:
                    queryWhere = buildQueryExact2(queryElements, isAndOperator);
                    break;
            }
            
            //query += "SELECT Movie_ID, ts_headline(MovieName, to_tsquery(";
            //query += querySelect;
            //query += ")), MovieName, ts_rank(tsvector, to_tsvector(";
            //query += querySelect;
            //query += ")) rank ";
            //query += "FROM movie ";
            //query += queryWhere;
            //query += " ORDER BY rank DESC";

            query = String.Format("SELECT movie_id, ts_headline(title, to_tsquery({0})), title, ts_rank(title_tsvector, to_tsquery ({1})) rank FROM movies WHERE {2} ORDER BY rank DESC", querySelect, querySelect, queryWhere);
            return query;
        }

        public string buildExactQueryText(List<String> queryElements, bool isAndOperator)
        {
            string query = "";
            string querySelect = buildQuerySelect(queryElements, isAndOperator);
            string queryWhere = buildQueryExact2(queryElements, isAndOperator);
            //query += "SELECT Movie_ID, ts_headline(MovieName, to_tsquery(";
            //query += querySelect;
            //query += ")), \nMovieName, ts_rank(tsvector, to_tsvector(" ;
            //query += querySelect;
            //query += ")) rank ";
            //query += "\nFROM movie ";
            //query += queryWhere;            
            query = String.Format("SELECT movie_id,\n ts_headline(title, to_tsquery({0})), title,\n ts_rank(title_tsvector, to_tsquery ({1})) rank \nFROM movies\n WHERE {2} \nORDER BY rank DESC", querySelect, querySelect, queryWhere);
            return query;
        }

        public List<string> divideQuery(string query)
        {
            if (query.Length > 0)
            {
                string[] queryElementsBasic = null;
                queryElementsBasic = query.Split('"');
                List<string> queryElementsFull = new List<string>();
                foreach (var item in queryElementsBasic)
                {
                    if (item.Length > 0)
                    {
                        var first = item.Substring(0, 1);
                        if (item.Substring(0,1).Equals(" "))
                        {
                            string[] queryElementsInside = item.Split(' ');
                            foreach (var item1 in queryElementsInside)
                            {
                                if (item1.Length>0)
                                {
                                    queryElementsFull.Add(item1);
                                }
                            }
                        }
                        else
                        {
                            queryElementsFull.Add("'" + item + "'");
                        }
                    }
                }
                return queryElementsFull;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region buildSelectQuery
        public string buildQuerySelect(List<string> queryElements, bool isAndOperator)
        {
            string operatorSign;
            operatorSign = (isAndOperator == true) ? "&" : "|";
            string newQuery = "'";

            for (var i = 0; i < queryElements.Count; i++)
            {
                if (queryElements[i][0].CompareTo('\'') != 0)
                {
                    newQuery += queryElements[i] + " " + operatorSign + " ";
                }
            }

            for (var i = 0; i < queryElements.Count; i++)
            {
                if (queryElements[i][0].CompareTo('\'') == 0)
                {
                    string[] subQueryElements = queryElements[i].Substring(1, queryElements[i].Length - 2).Split(' ');
                    string subQueryElement = "(";
                    foreach (var item in subQueryElements)
                    {
                        subQueryElement += item + " & ";
                    }
                    subQueryElement = subQueryElement.Substring(0, subQueryElement.Length - 3)+')';
                    queryElements[i] = subQueryElement;
                    newQuery += queryElements[i] + " " + operatorSign + " ";
                }
            }
            newQuery = newQuery.Substring(0, newQuery.Length - 3) + "'";

            return newQuery;
        }
        #endregion

        #region buildWhereQuery
        public string buildQueryExact2(List<string> queryElements, bool isAndOperator)
        {
            string operatorSign;
            operatorSign = (isAndOperator == true) ? "AND" : "OR";
            string newQuery = "";

            for (var i = 0; i < queryElements.Count; i++)
            {
                if (queryElements[i][0].CompareTo('(') != 0)
                {
                    queryElements[i] = "'%" + queryElements[i] + "%'";
                }
                else
                {
                    string subQuery = queryElements[i].Substring(1, queryElements[i].Length - 2);
                    subQuery = subQuery.Replace(" & ", " ");
                    subQuery = subQuery.Replace(" | ", " ");
                    queryElements[i] = "'%" + subQuery + "%'";
                }
                newQuery += "title LIKE " + queryElements[i] + " " + operatorSign + " ";
            }
            newQuery = newQuery.Substring(0, newQuery.Length - (2 + operatorSign.Length));
            return newQuery;
        }

        public string buildQueryDictionary(List<string> queryElements, bool isAndOperator)
        {
            string operatorSign;
            operatorSign = (isAndOperator == true) ? "AND" : "OR";
            string newQuery = "";

            for (var i = 0; i < queryElements.Count; i++)
            {
                if (queryElements[i][0].CompareTo('(') != 0)
                {
                    queryElements[i] = "'" + queryElements[i] + "'";
                }
                else
                {
                    string subQuery = queryElements[i].Substring(1, queryElements[i].Length - 2);
                    queryElements[i] = "'" + subQuery + "'";
                }
                newQuery += String.Format("title_tsvector @@ to_tsquery('english',{0}) {1} ", queryElements[i], operatorSign);
            }
            newQuery = newQuery.Substring(0, newQuery.Length - (2 + operatorSign.Length));
            return newQuery;
        }

        public string buildQueryFuzzy(List<string> queryElements, bool isAndOperator)
        {
            string operatorSign;
            operatorSign = (isAndOperator == true) ? "AND" : "OR";
            string newQuery = "";

            for (var i = 0; i < queryElements.Count; i++)
            {
                if (queryElements[i][0].CompareTo('(') != 0)
                {
                    queryElements[i] = "'" + queryElements[i] + "'";
                }
                else
                {
                    string subQuery = queryElements[i].Substring(1, queryElements[i].Length - 2);
                    subQuery = subQuery.Replace(" & ", " ");
                    subQuery = subQuery.Replace(" | ", " ");
                    queryElements[i] = "'" + subQuery + "'";
                }
                newQuery += String.Format("title % {0} {1} ", queryElements[i], operatorSign);
            }
            newQuery = newQuery.Substring(0, newQuery.Length - (2 + operatorSign.Length));
            return newQuery;
        }
        #endregion

        #region communicate with DB
        public List<Core.Movie> getResultsfromDB(string query, List<string> searchQuery, bool andIsTrue)
        {
            List<Core.Movie> getAllMovies = new List<Core.Movie>();

            string searchValue = buildQuerySelect(searchQuery, andIsTrue);
            searchValue = searchValue.Substring(1, searchValue.Length - 2);

            getAllMovies = database.getMoviesFiltered(query, searchValue);

            return getAllMovies;
        }

        public string addMovie(string movieName)
        {
            var result = database.addMovie(movieName);
            return result;
        }

        public Core.analyseResults getStatistics(DateTime startDate, DateTime endDate, bool byHour)
        {
            return database.getStatistics(startDate, endDate, byHour);
        }

        public Manager(Core.serverSetup connectionSetup)
        {
            this.database = new DAL.DBAccess(connectionSetup);
        }
        #endregion
    }
}
