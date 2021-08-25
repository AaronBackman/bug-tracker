using System;
using System.Collections.Generic;

namespace bug_tracker
{
    public interface IProjectMemberRepository
    {
        public List<ProjectMember> GetAll(Guid ProjectGuid, string email);
        public void Add(ProjectMember projectMember, Guid ProjectGuid, string email);

        public void Put(ProjectMember projectMember, Guid ProjectGuid, string email);
        public void Delete(Guid projectMemberGuid, string email);
    }
}