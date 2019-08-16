using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DnDBuilder.Controllers
{
    public class DnDBuilderController : ApiController
    {

        [HttpPost]
        [Route("DnDBuilder/getRaceList")]
        public String[] getRaceList()
        {
            DnD5eAPI dnd5eAPI = new DnD5eAPI();
            Races races = dnd5eAPI.getRaces();
            if (races == null)
            {
                return null;
            }

            int i = 0;
            String[] raceList = new String[races.results.Count];
            foreach (var result in races.results)
            {
                raceList[i] = result.name;
                i++;
            }

            //erase the retieve data
            dnd5eAPI = null;

            return raceList;
        }

        [HttpPost]
        [Route("DnDBuilder/getClassList")]
        public String[] getClassList()
        {
            DnD5eAPI dnd5eAPI = new DnD5eAPI();
            Classes classes = dnd5eAPI.getClasses();
            if (classes == null)
            {
                return null;
            }

            int i = 0;
            String[] classList = new String[classes.results.Count];
            foreach (var result in classes.results)
            {
                classList[i] = result.name;
                i++;
            }

            //erase the retieve data
            dnd5eAPI = null;

            return classList;
        }

        [HttpPost]
        [Route("DnDBuilder/getMaxLevel")]
        public int getMaxLevel()
        {
            return 20;
        }

        //Check whether json file contain Spell Casting
        [HttpPost]
        [Route("DnDBuilder/getClassInfo")]
        public JObject getClassInfo([FromBody] Dictionary<string, String> req)
        {
            DnD5eAPI dnd5eAPI = new DnD5eAPI();
            JObject classInfo = new JObject();
            classInfo = dnd5eAPI.getClassInfo(req["classes"]);

            //erase the retrieved data
            dnd5eAPI = null;

            return classInfo;
        }

        //get Hit Die point for HP computing
        [HttpPost]
        [Route("DnDBuilder/getRacialBonus")]
        public JObject getRacialBonus([FromBody] Dictionary<string, String> req)
        {
            DnD5eAPI dnd5eAPI = new DnD5eAPI();
            JObject raceBonus = new JObject();
            raceBonus = dnd5eAPI.getRacialBonus(req["races"]);

            //erase the retrieved data
            dnd5eAPI = null;

            return raceBonus;

        }

        //Process character creation
        [HttpPost]
        [Route("DnDBuilder/createHero")]
        public String createHero([FromBody] Dictionary<string, Object> req)
        {
            DnD5eAPI dnd5eAPI = new DnD5eAPI();
            Races races = dnd5eAPI.getRaces();
            if (races == null)
            {
                return "Fail to get races list from dnd5eapi";
            }
            Classes classes = dnd5eAPI.getClasses();
            if (classes == null)
            {
                return "Fail to get classes list from dnd5eapi";
            }

            //erase the retieve data
            dnd5eAPI = null;

            String json = JsonConvert.SerializeObject(req, Newtonsoft.Json.Formatting.Indented);
            DataStructure inputData = new DataStructure();
            try
            {
                inputData = JsonConvert.DeserializeObject<DataStructure>(json);
            }
            catch (JsonSerializationException)
            {
                return "incorect input form";
            }

            Validation validation = new Validation();
            validation.setInputData(inputData);
            if (validation.run(races, classes, true) != null)
            {
                return validation.getErrorMsg();
            }

            DBHandler dBHandler = new DBHandler();
            return dBHandler.createChar(inputData);
        }

        //Process character updating
        [HttpPost]
        [Route("DnDBuilder/updateHero")]
        public String updateHero([FromBody] Dictionary<string, Object> req)
        {
            DnD5eAPI dnd5eAPI = new DnD5eAPI();
            Races races = dnd5eAPI.getRaces();
            if (races == null)
            {
                return "Fail to get races list from dnd5eapi";
            }
            Classes classes = dnd5eAPI.getClasses();
            if (classes == null)
            {
                return "Fail to get classes list from dnd5eapi";
            }

            //erase the retieve data
            dnd5eAPI = null;

            String json = JsonConvert.SerializeObject(req, Newtonsoft.Json.Formatting.Indented);
            DataStructure inputData = new DataStructure();
            try
            {
                inputData = JsonConvert.DeserializeObject<DataStructure>(json);
            }
            catch (JsonSerializationException)
            {
                return "incorect input form";
            }

            Validation validation = new Validation();
            validation.setInputData(inputData);
            if (validation.run(races, classes, false) != null)
            {
                return validation.getErrorMsg();
            }

            DBHandler dBHandler = new DBHandler();
            return dBHandler.updateChar(inputData);
        }

        //Process character creation
        [HttpPost]
        [Route("DnDBuilder/deleteHero")]
        public String deleteHero([FromBody] Dictionary<string, string> req)
        {
            String name;
            try {
                name = req["name"];
            }
            catch (KeyNotFoundException) {
                return "DnDBuilder/deleteHero: The key of input json file is incorrect";
            }


            Validation validation = new Validation();
            String errorMsg = validation.checkName(name, false);
            if (errorMsg != null)
            {
                return errorMsg;
            }

            DBHandler dBHandler = new DBHandler();
            return dBHandler.deleteChar(name);
        }


        //load specific character
        [HttpPost]
        [Route("DnDBuilder/getCharInfo")]
        public JObject getCharInfo([FromBody] Dictionary<string, string> req)
        {
            DBHandler dBHandler = new DBHandler();
            String name;
            try {
                name = req["chName"];
            }
            catch (KeyNotFoundException)
            {
                JObject error = new JObject();
                error.Add("error_Msg", "DnDBuilder/getCharInfo: The key of input json file is incorrect");
                return error;
            }

            Validation validation = new Validation();
            String errorMsg = validation.checkName(name, false);
            if (errorMsg != null)
            {
                JObject error = new JObject();
                error.Add("error_Msg", errorMsg);
                return error;
            }

            errorMsg = dBHandler.loadChar(name);
            if (errorMsg != null)
            {
                return new JObject() { "error_Msg", errorMsg };
            }
            return dBHandler.getCharInfo();
        }

        //load bref detail for list in Main page
        [HttpPost]
        [Route("DnDBuilder/getBriefList")]
        public JArray getBriefList()
        {

            DBHandler dBHandler = new DBHandler();
            String errorMsg = dBHandler.loadCharList();
            if (errorMsg != null)
            {
                //return errorMsg;
                JArray error = new JArray();
                JObject msg = new JObject();
                msg.Add("error_Msg", errorMsg);
                error.Add(msg);
                return error;
            }
            return dBHandler.getCharList();
        }
    }
}
