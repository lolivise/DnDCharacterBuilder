using System;
using System.Collections.Generic;

namespace DnDBuilder.Controllers
{
    public class CharProperty
    {
        public String name { get; set; }
        public int age { get; set; }
        public String gender { get; set; }
        public String biograph { get; set; }
        public String chosenRace { get; set; }
        public String chosenClass { get; set; }
        public int level { get; set; }
        public int point { get; set; }
        public int[] userAB { get; set; }

        public CharProperty(Dictionary<string, object> dictionary)
        {
            this.name = (string)dictionary["name"];
            this.age = Convert.ToInt32(dictionary["age"]);
            this.gender = (string)dictionary["gender"];
            this.biograph = (string)dictionary["bio"];
            this.chosenRace = (string)dictionary["races"];
            this.chosenClass = (string)dictionary["classes"];
            this.level = Convert.ToInt32(dictionary["level"]);
            this.point = Convert.ToInt32(dictionary["point"]);
            this.userAB = new int[] { Convert.ToInt32(dictionary["userCON"]),
                                      Convert.ToInt32(dictionary["userDEX"]),
                                      Convert.ToInt32(dictionary["userSTR"]),
                                      Convert.ToInt32(dictionary["userCHA"]),
                                      Convert.ToInt32(dictionary["userINT"]),
                                      Convert.ToInt32(dictionary["userWIS"])};
        }
    }
}
