using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DnDBuilder.Controllers
{
    public class DnD5eAPI
    {
        public DnD5eAPI()
        {
        }

        //Check whether a key is exist
        public Boolean checkJsonKey(JObject jsonObj, String node)
        {

            if (jsonObj[node] != null)
            {
                return true;
            }
            return false;
        }

        public Races getRaces()
        {
            String url = "http://dnd5eapi.co/api/races";
            String data = getData(url);
            try {
                Races races = JsonConvert.DeserializeObject<Races>(data);
                return races;
            }
            catch (JsonSerializationException)
            {
                return null;
            }
            catch (JsonReaderException)
            {
                return null;
            }

        }

        //Retrieve class list and return the value
        public Classes getClasses()
        {
            String url = "http://dnd5eapi.co/api/classes";
            String data = getData(url);
            try
            {
                Classes classes = JsonConvert.DeserializeObject<Classes>(data);
                return classes;
            }
            catch (JsonSerializationException)
            {
                return null;
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        //Retrieve info of spellcasting and hit_die point from a class
        public JObject getClassInfo(String chooseClass) {
            JObject classInfo = new JObject();
            Classes classes = getClasses();
            if (classes == null)
            {
                classInfo.Add("error_Msg", "Fail to get classes list from dnd5eapi");
                return classInfo;
            }

            try
            {
                String entry = "";
                foreach (var result in classes.results)
                {
                    if (result.name.Equals(chooseClass))
                    {
                        entry = result.url;
                        JObject jsonObj = JObject.Parse(getData(entry));
                        if (checkJsonKey(jsonObj, "spellcasting"))
                        {
                            classInfo.Add("isSpellCaster", true);
                        }
                        else
                        {
                            classInfo.Add("isSpellCaster", false);
                        }

                        if (checkJsonKey(jsonObj, "hit_die"))
                        {
                            classInfo.Add("hitdie", jsonObj["hit_die"]);
                        }
                        else
                        {
                            classInfo.Add("error_Msg", "Fail to get hit die point");
                        }
                        return classInfo;

                    }
                }
                //return error message if class is not found
                classInfo.Add("error_Msg", chooseClass + " is not in the list!");
                return classInfo;
            }
            catch (KeyNotFoundException)
            {
                classInfo.Add("error_Msg", "DnDBuilder/getClassInfo: The key of input json file is incorrect");
                return classInfo;
            }
            catch (WebException)
            {
                classInfo.Add("error_Msg", "DnDBuilder/getClassInfo: Fail to get " + chooseClass + " data from dnd5eapi.");
                return null;
            }
        }

        //Retieve the reacial bonus and return the result
        public JObject getRacialBonus(String chooseRace) {
            Races races = getRaces();
            JObject raceBonus = new JObject();
            if (races == null)
            {
                raceBonus.Add("error_Msg", "Fail to get races list from dnd5eapi");
                return raceBonus;
            }
            try
            {
                String entry = "";
                String node = "ability_bonuses";
                foreach (var result in races.results)
                {
                    if (result.name.Equals(chooseRace))
                    {
                        entry = result.url;
                        JObject jsonObj = JObject.Parse(getData(entry));
                        if (checkJsonKey(jsonObj, node))
                        {
                            //int[] res = new int[6];
                            String res = "[";
                            for (int i = 0; i < 5; i++)
                            {
                                res = res + (string)jsonObj[node][i] + ",";
                            }
                            res = res + (string)jsonObj[node][5] + "]";
                            raceBonus.Add("racialBonus", JToken.Parse(res));
                        }
                        else
                        {
                            raceBonus.Add("error_Msg", "Racial bonus is not found in " + chooseRace);
                        }
                        return raceBonus;
                    }
                }
                //Add an error message if race is not in the list
                raceBonus.Add("error_Msg", chooseRace + " is not in the list!");
                return raceBonus;
            }
            catch (KeyNotFoundException)
            {
                raceBonus.Add("error_Msg", "DnDBuilder/getRacialBonus: The key of input json file is incorrect");
                return raceBonus;
            }
            catch (WebException)
            {
                raceBonus.Add("error_Msg", "DnDBuilder/getRacialBonus: Fail to get " + chooseRace + " data from dnd5eapi.");
                return null;
            }

        }

        //Get the data from the input url
        public String getData(String url)
        {
            try
            {
                //setuo the calling method
                String strurl = String.Format(url);
                WebRequest requestObject = WebRequest.Create(strurl);
                requestObject.Method = "GET";
                HttpWebResponse responseObject = null;
                responseObject = (System.Net.HttpWebResponse)requestObject.GetResponse();

                String strresult = null;
                using (Stream stream = responseObject.GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(stream);
                    strresult = streamReader.ReadToEnd();
                    streamReader.Close();
                }
                return strresult;
            }
            catch (WebException)
            {
                return null;
            }

        }
    }
}
