using System;
using System.Collections.Generic;

namespace bug_tracker
{
    public interface IProjectMemberRepository
    {
        public List<ProjectMember> GetAll(Guid ProjectGuid, string email);
        public void Add(ProjectMember projectMember);

        public void Put(ProjectMember projectMember);
        public void Delete(ProjectMember projectMember);
    }
}