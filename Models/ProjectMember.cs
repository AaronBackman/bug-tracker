using System;

namespace bug_tracker
{
    // part of a project
    public class ProjectMember
    {
        public string Username { get; set; }
        public string NickName {get; set;}
        public Role Role { get; set; }
        public int ProjectId {get; set;}
    }
}
