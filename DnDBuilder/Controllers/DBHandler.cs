using System;
using Mono.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DnDBuilder.Controllers
{
    public class DBHandler
    {

        private const String dbName = "DnDBuilder.sqlite";
        private JArray charList;
        private JObject charInfo;

        public DBHandler()
        {

        }

        //Create
        public String createChar(DataStructure inputData) 
        {
            try
            {
                using (SqliteConnection m_dbConn = new SqliteConnection("Data Source=" + dbName + ";Version=3;"))
                {
                    m_dbConn.Open();
                    SqliteCommand insertSQL;
                    String columnName = "(name, age, gender, bio, races, classes, level, chCON, chDEX, chSTR, chCHA, chINT, chWIS)";
                    String columnValue = "(@name, @age, @gender, @bio, @races, @classes, @level, @chCON, @chDEX, @chSTR, @chCHA, @chINT, @chWIS)";
                    insertSQL = new SqliteCommand("INSERT INTO characters" + columnName + " VALUES" + columnValue, m_dbConn); //set up the insert command
                    insertSQL.Parameters.Add(new SqliteParameter("name", inputData.name));
                    insertSQL.Parameters.Add(new SqliteParameter("age", inputData.age));
                    insertSQL.Parameters.Add(new SqliteParameter("gender", inputData.gender));
                    insertSQL.Parameters.Add(new SqliteParameter("bio", inputData.bio));
                    insertSQL.Parameters.Add(new SqliteParameter("races", inputData.races));
                    insertSQL.Parameters.Add(new SqliteParameter("classes", inputData.classes));
                    insertSQL.Parameters.Add(new SqliteParameter("level", inputData.level));
                    insertSQL.Parameters.Add(new SqliteParameter("chCON", inputData.userAB[0]));
                    insertSQL.Parameters.Add(new SqliteParameter("chDEX", inputData.userAB[1]));
                    insertSQL.Parameters.Add(new SqliteParameter("chSTR", inputData.userAB[2]));
                    insertSQL.Parameters.Add(new SqliteParameter("chCHA", inputData.userAB[3]));
                    insertSQL.Parameters.Add(new SqliteParameter("chINT", inputData.userAB[4]));
                    insertSQL.Parameters.Add(new SqliteParameter("chWIS", inputData.userAB[5]));

                    insertSQL.ExecuteNonQuery(); //execute the command
                    m_dbConn.Close();
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                return e.Message;
            }

            return null;
        }

        //Save
        public String updateChar(DataStructure inputData)
        {
            try
            {
                using (SqliteConnection m_dbConn = new SqliteConnection("Data Source=" + dbName + ";Version=3;"))
                {
                    m_dbConn.Open();
                    SqliteCommand updateSQL;
                    updateSQL = new SqliteCommand("UPDATE  characters SET name= @name, " +
                    	                                                 "age = @age, "+
                                                                         "gender = @gender, "+
                                                                         "bio = @bio, "+
                                                                         "races = @races, "+
                                                                         "classes = @classes, "+
                                                                         "level = @level, "+
                                                                         "chCON = @chCON, "+
                                                                         "chDEX = @chDEX, "+
                                                                         "chSTR = @chSTR, "+
                                                                         "chCHA = @chCHA, "+
                                                                         "chINT = @chINT, "+
                                                                         "chWIS = @chWIS "+
                                                                         "where name = @confirmName",m_dbConn);
                    updateSQL.Parameters.Add(new SqliteParameter("name", inputData.name));
                    updateSQL.Parameters.Add(new SqliteParameter("age", inputData.age));
                    updateSQL.Parameters.Add(new SqliteParameter("gender", inputData.gender));
                    updateSQL.Parameters.Add(new SqliteParameter("bio", inputData.bio));
                    updateSQL.Parameters.Add(new SqliteParameter("races", inputData.races));
                    updateSQL.Parameters.Add(new SqliteParameter("classes", inputData.classes));
                    updateSQL.Parameters.Add(new SqliteParameter("level", inputData.level));
                    updateSQL.Parameters.Add(new SqliteParameter("chCON", inputData.userAB[0]));
                    updateSQL.Parameters.Add(new SqliteParameter("chDEX", inputData.userAB[1]));
                    updateSQL.Parameters.Add(new SqliteParameter("chSTR", inputData.userAB[2]));
                    updateSQL.Parameters.Add(new SqliteParameter("chCHA", inputData.userAB[3]));
                    updateSQL.Parameters.Add(new SqliteParameter("chINT", inputData.userAB[4]));
                    updateSQL.Parameters.Add(new SqliteParameter("chWIS", inputData.userAB[5]));
                    updateSQL.Parameters.Add(new SqliteParameter("confirmName", inputData.name));

                    updateSQL.ExecuteNonQuery();
                    m_dbConn.Close();
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                return e.Message;
            }

            return null;
        }

        //delete
        public String deleteChar(String name) {
            try
            {
                using (SqliteConnection m_dbConn = new SqliteConnection("Data Source=" + dbName + ";Version=3;"))
                {
                    m_dbConn.Open();
                    SqliteCommand deleteSQL;
                    deleteSQL = new SqliteCommand("DELETE FROM characters WHERE name = @name", m_dbConn);
                    deleteSQL.Parameters.Add(new SqliteParameter("name", name));
                    deleteSQL.ExecuteNonQuery();
                    m_dbConn.Close();
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                return e.Message;
            }

            return null;
        }

        //Load specific character
        public String loadChar(String name)
        {
            try
            {
                using (SqliteConnection m_dbConn = new SqliteConnection("Data Source=" + dbName + ";Version=3;"))
                {
                    m_dbConn.Open();
                    charInfo = new JObject();
                    string sql = "SELECT *  FROM characters WHERE name = @name";
                    SqliteCommand command = new SqliteCommand(sql, m_dbConn);
                    command.Parameters.Add(new SqliteParameter("name", name));
                    SqliteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {

                        charInfo.Add("name",(string)reader["name"]);
                        charInfo.Add("age", (int)reader["age"]);
                        charInfo.Add("gender", (string)reader["gender"]);
                        charInfo.Add("bio", (string)reader["bio"]);
                        charInfo.Add("races", (string)reader["races"]);
                        charInfo.Add("classes", (string)reader["classes"]);
                        charInfo.Add("level", (int)reader["level"]);
                        String temp = "["+ (int)reader["chCON"] + ","
                                         + (int)reader["chDEX"] + ","
                                         + (int)reader["chSTR"] + ","
                                         + (int)reader["chCHA"] + ","
                                         + (int)reader["chINT"] + ","
                                         + (int)reader["chWIS"] + "]";
                        charInfo.Add("userAB", JToken.Parse(temp.ToString()));
                    }
                    m_dbConn.Close();
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                return e.Message;
            }
            catch (JsonSerializationException e)
            {
                return e.Message;
            }
            catch (JsonReaderException e)
            {
                return e.Message;
            }

            return null;
        }

        //Load character list for main page
        public String loadCharList()
        {
            try
            {
                using (SqliteConnection m_dbConn = new SqliteConnection("Data Source=" + dbName + ";Version=3;"))
                {
                    m_dbConn.Open();
                    charList = new JArray();
                    string sql = "select name,races, classes, level  from characters";
                    SqliteCommand command = new SqliteCommand(sql, m_dbConn);
                    SqliteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        JObject charpartInfo = new JObject();
                        charpartInfo.Add("name", (string)reader["name"]);
                        charpartInfo.Add("races", (string)reader["races"]);
                        charpartInfo.Add("classes", (string)reader["classes"]);
                        charpartInfo.Add("level", (int)reader["level"]);
                        charList.Add(charpartInfo);
                    }
                    m_dbConn.Close();
                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                return e.Message;
            }
            catch (JsonSerializationException e)
            {
                return e.Message;
            }
            catch (JsonReaderException e)
            {
                return e.Message;
            }

            return null;
        }

        public JObject getCharInfo()
        {
            return charInfo;
        }

        public JArray getCharList()
        {
            return charList;
        }
    }
}
