using System;

namespace bug_tracker
{
    // part of a project
    public class ProjectMember
    {
        public string Username { get; set; }
        public Role ProjectRole { get; set; }
        public int ProjectId {get; set;}
    }
}
