using Dapper;
using System;
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

        public List<Project> GetAll(string email)
        {

            using (IDbConnection dbConnection = Connection)
            {
                string sql = 
                    @"SELECT Projects.ProjectName, Projects.ProjectGUID, Users.Email
                    FROM Projects
                    INNER JOIN Users ON Users.Id=Projects.OwnerId
                    WHERE Projects.Id =
                        (SELECT Projects.Id
                        FROM Projects
                        INNER JOIN ProjectMembers
                        ON Projects.Id= ProjectMembers.ProjectId
                        INNER JOIN Users
                        ON ProjectMembers.UserId=Users.Id
                        WHERE Users.Email=@Email
                        )";
                dbConnection.Open();

                List<Project> projects = dbConnection.Query<Project>(sql, new {Email = email}).ToList<Project>();

                return projects;
            }
        }

        public void Add(Project project, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                dbConnection.Open();

                int ownerId = new UserRepository().GetId(email);

                string sql = @"INSERT INTO Projects (ProjectName) VALUES(@ProjectName, @OwnerId)";
                dbConnection.Execute(sql, new {ProjectName = project.ProjectName, OwnerId = ownerId});
            }
        }

        public void Put(Project project, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql =
                @"UPDATE Projects
                SET ProjectName=@ProjectName
                WHERE ProjectGUID=@ProjectGUID
                AND EXISTS(
                    SELECT *
                    FROM Users
                    INNER JOIN Projects
                    ON Users.Id=Projects.OwnerId
                    WHERE Users.Email=@Email
                )";
                dbConnection.Open();

                dbConnection.Execute(sql, new {ProjectName = project.ProjectName, ProjectGUID = project.ProjectGUID, Email = email});
            }
        }

        public void Delete(Guid projectGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql =
                @"DELETE FROM Projects
                WHERE ProjectGUID=@ProjectGUID
                AND EXISTS(
                    SELECT *
                    FROM Users
                    INNER JOIN Projects
                    ON Users.Id=Projects.OwnerId
                    WHERE Users.Email=@Email
                )";
                dbConnection.Open();

                dbConnection.Execute(sql, new {ProjectGUID = projectGuid, Email = email});
            }
        }

        public int GetId(Guid guid)
        {
            using (IDbConnection dbConnection = Connection) {
                dbConnection.Open();

                string sql = @"SELECT Projects.Id FROM Projects WHERE Projects.ProjectGUID=@Guid";
                int id = dbConnection.Query<int>(sql, new {Guid = guid}).FirstOrDefault();

                return id;
            }
        }
    }
}