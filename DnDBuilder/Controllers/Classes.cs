using System;
using System.Collections.Generic;

namespace DnDBuilder.Controllers
{
    public class Classes
    {
        public int count { get; set; }
        public List<Class> results { get; set; }
    }

    public class Class
    {
        public string name { get; set; }
        public string url { get; set; }
    }
}
