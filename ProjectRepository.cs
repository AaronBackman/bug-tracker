using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace bug_tracker
{
    public class ProjectRepository : IProjectRepository
    {
        private string connectionString;
        public IDbConnection Connection
        {
            get{
                return new SqlConnection(connectionString);
            }
        }

        public ProjectRepository()
        {
            connectionString = "data source=carbon;initial catalog=BugTrackerDB;user id=BugTracker;password=Password123";
        }

        public Project GetById(int projectId)
        {

            using (IDbConnection dbConnection = Connection)
            {
                string sql = @"SELECT * FROM Projects WHERE ProjectId=@ProjectId";
                dbConnection.Open();

                Project project = dbConnection.Query<Project>(sql, new {ProjectId = projectId}).FirstOrDefault();
                if (project != null) {
                    string projectsSql = @"SELECT * FROM ProjectMembers INNER JOIN Projects ON ProjectMembers.ProjectId = Projects.ProjectId";
                    project.ProjectMembers = dbConnection.Query<ProjectMember>(projectsSql).ToList();
                }

                return project;
            }
        }

        public void Add(Project project)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql = @"INSERT INTO Projects (ProjectId, ProjectName) VALUES(@ProjectId,@ProjectName)";
                dbConnection.Open();

                dbConnection.Execute(sql, project);
            }
        }

        public void Put(Project project)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql = @"UPDATE Projects SET ProjectName=@ProjectName WHERE ProjectId=@ProjectId";
                dbConnection.Open();

                dbConnection.Execute(sql, project);
            }
        }

        public void Delete(Project project)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql = @"DELETE FROM Projects WHERE ProjectId=@ProjectId";
                dbConnection.Open();

                dbConnection.Execute(sql, project);
            }
        }
    }
}