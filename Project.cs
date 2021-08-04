using System;
using System.Collections.Generic;

namespace bug_tracker
{
    public class Project
    {
        public List<ProjectMember> ProjectMembers {get; set;}
        public string ProjectName {get; set;}
        public string ProjectCreator {get; set;}
        public int ProjectId {get; set;}
    }
}
