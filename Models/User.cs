using System;
using System.Collections.Generic;

namespace bug_tracker
{
    public class User
    {
        public string Username { get; set; }
        public string Nickname { get; set; }
        // a list of project IDs
        public List<int?> Projects {get; set;}
    }
}
