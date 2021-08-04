using System;
using System.Collections.Generic;

namespace bug_tracker
{
    public class User
    {
        public string Username { get; set; }
        public string Nickname { get; set; }
        public List<int> Projects {get; set;}
    }
}
