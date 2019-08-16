using System;
using System.Collections.Generic;

namespace DnDBuilder.Controllers
{
    public class Races
    {
        public int count { get; set; }
        public List<Race> results { get; set; }
    }

    public class Race
    {
        public string name { get; set; }
        public string url { get; set; }
    }
}
