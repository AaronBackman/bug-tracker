using Dapper;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace bug_tracker
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private string connectionString;
        public IDbConnection Connection
        {
            get{
                return new SqlConnection(connectionString);
            }
        }

        public ProjectMemberRepository()
        {
            connectionString = "data source=carbon;initial catalog=BugTrackerDB;user id=BugTracker;password=Password123";
        }

        public List<ProjectMember> GetAll(Guid ProjectGuid, string email)
        {

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                string sql = 
                    @"SELECT ProjectMembers.ProjectRole, ProjectMembers.ProjectMemberGUID, Users.Nickname, Users.Email
                    FROM ProjectMembers
                    INNER JOIN Users ON Users.Id=ProjectMembers.UserId
                    WHERE ProjectMembers.ProjectGUID=@ProjectGUID
                    AND EXISTS
                        (SELECT *
                        FROM Users
                        INNER JOIN ProjectMembers
                        ON Users.Id=ProjectMembers.Id
                        WHERE ProjectMembers.ProjectGUID=@ProjectGUID AND Users.Email=@Email
                        )";

                List<ProjectMember> projects = dbConnection.Query<ProjectMember>(sql, new {ProjectGUID = ProjectGuid, Email = email}).ToList<ProjectMember>();

                return projects;
            }
        }

        public void Add(ProjectMember projectMember, Guid projectGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                dbConnection.Open();

                int userId = new UserRepository().GetId(projectMember.Email);
                int projectId = new ProjectRepository().GetId(projectGuid);

                string sql =
                @"INSERT INTO ProjectMembers (ProjectRole, ProjectId, UserId)
                VALUES(@ProjectRole, @ProjectId, @UserId)
                WHERE EXISTS(
                    SELECT *
                    FROM Users
                    INNER JOIN ProjectMembers
                    ON Users.Id=ProjectMembers.UserId
                    WHERE Users.Email=@Email AND ProjectMembers.ProjectRole < @ProjectRole
                )";

                dbConnection.Execute(sql, new {ProjectRole = projectMember.ProjectRole, ProjectId = projectId, UserId = userId, Email = email});
            }
        }

        public void Put(ProjectMember projectMember, Guid ProjectGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql =
                @"UPDATE ProjectMembers AS P1
                SET P1.ProjectRole=@ProjectRole
                WHERE P1.ProjectMemberGUID=@ProjectMemberGUID
                AND EXISTS(
                    SELECT *
                    FROM Users
                    INNER JOIN ProjectMembers as P2
                    ON Users.Id=P2.UserId
                    WHERE Users.Email=@Email AND P2.ProjectRole < @ProjectRole AND P2.ProjectRole < P1.ProjectRole
                )";
                dbConnection.Open();

                dbConnection.Execute(sql, new {ProjectMemberGUID = projectMember.ProjectMemberGUID, Email = email});
            }
        }

        public void Delete(Guid projectMemberGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql =
                @"DELETE FROM ProjectMembers AS P1
                WHERE P1.ProjectMemberGUID=@ProjectMemberGUID
                AND EXISTS(
                    SELECT *
                    FROM Users
                    INNER JOIN ProjectMembers as P2
                    ON Users.Id=P2.UserId
                    WHERE Users.Email=@Email AND P2.ProjectRole < P1.ProjectRole
                )";
                dbConnection.Open();

                dbConnection.Execute(sql, new {ProjectMemberGUID = projectMemberGuid, Email = email});
            }
        }
    }
}