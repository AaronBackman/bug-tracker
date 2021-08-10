using System.Collections.Generic;

namespace bug_tracker
{
    public interface IProjectRepository
    {
        public Project GetById(int projectId);
        public void Add(Project project);

        public void Put(Project project);
        public void Delete(Project project);
    }
}