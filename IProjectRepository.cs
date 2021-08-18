using System;
using System.Collections.Generic;

namespace bug_tracker
{
    public interface IProjectRepository
    {
        public List<Project> GetAll(string email);
        public void Add(Project project, string email);

        public void Put(Project project);
        public void Delete(Project project);

        public int GetId(Guid guid);
    }
}