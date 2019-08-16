using System;
using System.Collections.Generic;

namespace DnDBuilder.Controllers
{
    public class DataStructure
    {
        public string name { get; set; }
        public int age { get; set; }
        public string gender { get; set; }
        public string bio { get; set; }
        public string races { get; set; }
        public string classes { get; set; }
        public int level { get; set; }
        public List<int> userAB { get; set; }
    }
}
