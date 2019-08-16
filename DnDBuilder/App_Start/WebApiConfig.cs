using System.IO;
using System.Web.Http;
using Mono.Data.Sqlite;

namespace DnDBuilder
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // DO NOT REMOVE THIS LINE
            config.MapHttpAttributeRoutes();
            // avoid convention - based routing , hence commented out
            // config . Routes . MapHttpRoute (
            // name : " DefaultApi " ,
            // routeTemplate : "api /{ controller }/{ id }" ,
            // defaults : new { id = RouteParameter . Optional }
            // );
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.
            SupportedMediaTypes.Clear();
            config.Formatters.Insert(0, new System.Net.Http.Formatting.
            JsonMediaTypeFormatter());
            // Apply this GLOBALLY so that we don 't have to be bothered
            //with it during other JSON operations
            Newtonsoft.Json.JsonConvert.DefaultSettings = () => new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.
            ReferenceLoopHandling.Ignore
            };
            /* If we don 't do this then JSON will send dates in a local - time
            format that is not consistently interpreted by Chrome and IE
            ( the default format misses defining the timezone , so it is USELESS .
            This one will always be UTC )*/
            config.Formatters.JsonFormatter.SerializerSettings.
            DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            createNewDatabase();
        }

        private static void createNewDatabase()
        {
            try
            {
                //check is the database file already exists
                if (!File.Exists("DnDBuilder.sqlite"))
                {
                    //create the database
                    SqliteConnection.CreateFile("DnDBuilder.sqlite");

                    //using a connection object
                    using (SqliteConnection m_dbConn = new SqliteConnection("Data Source=DnDBuilder.sqlite;Version=3;"))
                    {
                        m_dbConn.Open();
                        string columnName = "name varchar(20) unique";
                        string columnAge = "age INT";
                        string columnGender = "gender varchar(10)";
                        string columnbio = "bio varchar(500)";
                        string columnRace = "races varchar(20)";
                        string columnClass = "classes varchar(20)";
                        string columnLevel = "level INT";
                        string columnABScore = "chCON INT, chDEX INT, chSTR INT, chCHA INT, chINT INT, chWIS INT";
                        // the SQL string
                        string sql = "create table characters ("+ columnName + "," + 
                                                                  columnAge + "," +
                                                                  columnGender + "," +
                                                                  columnbio + "," +
                                                                  columnRace + "," +
                                                                  columnClass + "," +
                                                                  columnLevel + "," +
                                                                  columnABScore  + ")";
                        SqliteCommand command = new SqliteCommand(sql, m_dbConn); // create the command
                        command.ExecuteNonQuery(); // execute the query
                        m_dbConn.Close();
                    }
                }
            }
            catch (SqliteException e)
            {
                //exception handling goes here
            }

        }
    }
}
