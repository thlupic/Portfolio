using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Npgsql;
using System.Data;

namespace Project1.DAL
{
    public class DBAccess
    {
        //
        // GET: /DBAccess/
        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();

        //private static string server = "192.168.56.12";
        //private static string port = "5432";
        //private static string username = "postgres";
        //private static string password = "reverse";
        //private static string DB = "Movies";

        private static string server { get; set; }
        private static string port { get; set; }
        private static string username { get; set; }
        private static string password { get; set; }
        private static string DB { get; set; }

        static Core.serverSetup connectionData = new Core.serverSetup();
        
        public static void setConnection(Core.serverSetup connectionSetup)
        {
            server = connectionSetup.server;
            port = connectionSetup.port;
            username = connectionSetup.username;
            password = connectionSetup.password;
            DB = connectionSetup.DB;
        }

        private static NpgsqlConnection openConnection()//string server, string port, string username, string password, string DB)
        {
            // PostgeSQL-style connection string
            if (connectionData.server==null) connectionData = connectionData.setDefault();
            setConnection(connectionData);
            server = connectionData.server;
            port = connectionData.port;
            username = connectionData.username;
            password = connectionData.password;
            DB = connectionData.DB;
            string connstring = String.Format("Server={0};Port={1};" +
                "User Id={2};Password={3};Database={4};",
                server, port, username, password, DB);
            // Making connection with Npgsql provider
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            return conn;
        }

        private static NpgsqlConnection conn = openConnection();//server, port, username, password, DB);

        public List<Core.Movie> getAllMovies()
        {
            conn.Open();
            List<Core.Movie> movies = new List<Core.Movie>();
            string sql = "SELECT * FROM movies";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            for (var i=0;i<dt.Rows.Count;i++)
            {
                Core.Movie movie = new Core.Movie();
                var item = dt.Rows[i];
                var array = item.ItemArray;
                var ID = array[0];
                var name = array[1].ToString();
                movie.name = name;
                movie.ID = (int)ID;
                movies.Add(movie);
            }
            conn.Close();
            return movies;
        }

        public List<Core.Movie> getMoviesFiltered(string query, string searchValue)
        {
            conn.Open();
            List<Core.Movie> movies = new List<Core.Movie>();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                Core.Movie movie = new Core.Movie();
                var columns = dt.Columns;
                var item = dt.Rows[i];
                var array = item.ItemArray;
                var ID = array[0];
                var name = array[1].ToString();
                var rank = array[3].ToString();
                movie.name = name;
                movie.ID = (int)ID;
                movie.rank = Convert.ToDouble(rank);
                movies.Add(movie);
            }
            rememberSearch(searchValue);
            conn.Close();
            return movies;
        }

        public string addMovie(string movieName)
        {
            conn.Open();
            string query = String.Format("INSERT INTO movies VALUES (DEFAULT,'{0}',to_tsvector('english','{1}'))", movieName, movieName);
            try
            {
                var NpAdapter = new NpgsqlDataAdapter();
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
                NpgsqlCommand insert = new NpgsqlCommand(query, conn);
                //NpAdapter.InsertCommand = insert;
                insert.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            conn.Close();
            return "Zapis uspješno dodan";
        }

        public void rememberSearch(string searchValue)
        {
            string query = String.Format("INSERT INTO search VALUES (DEFAULT,'{0}',CURRENT_TIMESTAMP)", searchValue);
            var NpAdapter = new NpgsqlDataAdapter();
            NpgsqlCommand insert = new NpgsqlCommand(query, conn);
            //NpAdapter.InsertCommand = insert;
            insert.ExecuteNonQuery();
        }

        public Core.analyseResults getStatistics(DateTime startDate, DateTime endDate, bool byHour)
        {
            conn.Open();
            Core.analyseResults results = new Core.analyseResults();
            string query = "";

            if (!byHour)
            {
                string columnsDates = "";
                string querytemp = String.Format("CREATE TEMP TABLE dani (dan timestamp)");
                var NpAdapter = new NpgsqlDataAdapter();
                try
                {
                    NpgsqlCommand createTemp = new NpgsqlCommand(querytemp, conn);
                    createTemp.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    var ex = e;
                    conn.Close();
                }
                for (var day = startDate.Date; day.Date <= endDate.Date; day = day.AddDays(1))
                {
                    string dateString = String.Format("d{0}{1}{2}", day.Day, day.Month, day.Year);
                    try
                    {
                        string dateString2 = "\'" + day.ToString("yyyy-MM-dd") + "\'";
                        string queryTemp2 = String.Format("INSERT INTO dani (dan) VALUES (CAST({0} AS timestamp))", dateString2);
                        NpgsqlCommand insertTemp = new NpgsqlCommand(queryTemp2, conn);
                        insertTemp.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        var ex = e;
                        conn.Close();
                    }
                    columnsDates += String.Format("{0} INT, ", dateString);
                }
                columnsDates = columnsDates.Substring(0, columnsDates.Length - 2);
                query = String.Format("SELECT * FROM CROSSTAB('SELECT CAST(searchValue as text) AS query, CAST(EXTRACT(epoch FROM timestamp::timestamp::date) as int), CAST(COUNT (*) AS int) FROM search WHERE timestamp BETWEEN to_timestamp({0}) AND to_timestamp({1}) GROUP BY query, CAST(EXTRACT(epoch from timestamp::timestamp::date) as int) ORDER BY query, CAST(EXTRACT(epoch from timestamp::timestamp::date) as int)','SELECT CAST(EXTRACT (epoch FROM dan::date) as int) FROM dani ORDER BY dan') AS pivotTable(query TEXT, {2}) ORDER BY query", ConvertToTimestamp(startDate), ConvertToTimestamp(endDate), columnsDates);
            }
            else
            {
                string querytemp = String.Format("CREATE TEMP TABLE hours (hour int)");
                var NpAdapter = new NpgsqlDataAdapter();
                try
                {
                    NpgsqlCommand createTemp = new NpgsqlCommand(querytemp, conn);
                    createTemp.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    var ex = e;
                    conn.Close();
                }
                for (var i = 0; i < 24; i++)
                {
                    try
                    {
                        string queryTemp2 = String.Format("INSERT INTO hours (hour) VALUES (CAST({0} AS int))", i.ToString());
                        NpgsqlCommand insertTemp = new NpgsqlCommand(queryTemp2, conn);
                        insertTemp.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        var ex = e;
                        conn.Close();
                    }
                }
                query = String.Format("SELECT * FROM CROSSTAB('SELECT CAST(searchValue as text) AS query, CAST(EXTRACT(hour FROM timestamp) as int), CAST(COUNT (*) AS int) FROM search WHERE timestamp BETWEEN to_timestamp({0}) AND to_timestamp({1}) GROUP BY query, CAST(EXTRACT(hour from timestamp) as int) ORDER BY query, CAST(EXTRACT(hour from timestamp) as int)','SELECT hour FROM hours ORDER BY hour') AS pivotTable(query TEXT, h0 INT, h1 INT, h2 INT, h3 INT, h4 INT, h5 INT, h6 INT, h7 INT, h8 INT, h9 INT, h10 INT, h11 INT, h12 INT, h13 INT, h14 INT, h15 INT, h16 INT, h17 INT, h18 INT, h19 INT, h20 INT, h21 INT, h22 INT, h23 INT) ORDER BY query", ConvertToTimestamp(startDate), ConvertToTimestamp(endDate));
            }

            try
            {
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
                ds.Reset();
                da.Fill(ds);
                dt = ds.Tables[0];
                var columns = dt.Columns;
                for (var j = 0; j < columns.Count; j++)
                {
                    results.columns.Add(columns[j].Caption);
                }
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    Core.searchResults result = new Core.searchResults();
                    var item = dt.Rows[i];
                    var array = item.ItemArray;
                    result.query = array[0].ToString();
                    for (var k = 1; k < columns.Count; k++)
                    {
                        string countValue = array[k].ToString();
                        int count = 0;
                        if (!string.IsNullOrEmpty(countValue)) count = Convert.ToInt32(countValue);
                        result.values.Add(Convert.ToInt32(count));                        
                    }
                    results.results.Add(result);
                }
                results.query = query;
            }
            catch (Exception e)
            {
                var ex = e;
                conn.Close();
            }

            conn.Close();

            return results;
        }

        private static double ConvertToTimestamp(DateTime value)
        {
            //unix timestamp
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            return (double)span.TotalSeconds;
        }

        public DBAccess(Core.serverSetup connectionData)
        {
            setConnection(connectionData);
        }
    }   
}
