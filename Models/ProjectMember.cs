using System;

namespace bug_tracker
{
    // part of a project
    public class ProjectMember
    {
        public string Nickname { get; set; }
        public string Email {get; set;}
        public Role ProjectRole { get; set; }
        public Guid ProjectMemberGUID {get; set;}
    }
}
