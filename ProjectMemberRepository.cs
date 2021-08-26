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

        public List<ProjectMember> GetAll(Guid projectGuid, string email)
        {

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                int userId = new UserRepository().GetId(email);
                int projectId = new ProjectRepository().GetId(projectGuid);

                string sql = 
                    @"SELECT ProjectMembers.ProjectRole, ProjectMembers.ProjectMemberGUID, Users.Nickname, Users.Email
                    FROM ProjectMembers
                    INNER JOIN Users ON Users.Id=ProjectMembers.UserId
                    WHERE ProjectMembers.ProjectId=@ProjectId
                    AND EXISTS
                        (SELECT *
                        FROM Users
                        INNER JOIN ProjectMembers
                        ON Users.Id=ProjectMembers.UserId
                        WHERE ProjectMembers.ProjectId=@ProjectId AND Users.Id=@Id
                        )";

                List<ProjectMember> projects = dbConnection.Query<ProjectMember>(sql, new {ProjectId = projectId, Id = userId}).ToList<ProjectMember>();

                return projects;
            }
        }

        public ProjectMember Add(ProjectMember projectMember, Guid projectGuid, string email, bool isOwner)
        {
            using (IDbConnection dbConnection = Connection) {
                dbConnection.Open();

                int newUserId = new UserRepository().GetId(projectMember.Email);
                int userId = new UserRepository().GetId(email);
                int projectId = new ProjectRepository().GetId(projectGuid);
                string sql;

                if (!isOwner) {
                    sql =
                    @"INSERT INTO ProjectMembers (ProjectRole, ProjectId, UserId)
                    OUTPUT INSERTED.ProjectMemberGUID
                    VALUES(@ProjectRole, @ProjectId, @NewUserId)
                    SELECT *
                    FROM ProjectMembers
                    WHERE ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.UserId=@UserId AND ProjectMembers.ProjectRole < @ProjectRole";
                }
                else {
                    sql =
                    @"INSERT INTO ProjectMembers (ProjectRole, ProjectId, UserId)
                    OUTPUT INSERTED.ProjectMemberGUID
                    VALUES(@ProjectRole, @ProjectId, @NewUserId)
                    ";
                }

                projectMember.ProjectMemberGUID = dbConnection.ExecuteScalar<Guid>(sql, new {ProjectRole = projectMember.ProjectRole, ProjectId = projectId, NewUserId = newUserId, UserId = userId});

                return projectMember;
            }
        }

        public void Put(ProjectMember projectMember, Guid projectGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                
                int updatedUserId = new UserRepository().GetId(projectMember.Email);
                int userId = new UserRepository().GetId(email);
                int projectId = new ProjectRepository().GetId(projectGuid);

                // cant change your own role
                if (updatedUserId == userId) {
                    return;
                }

                string sql =
                @"UPDATE ProjectMembers
                SET ProjectRole=@ProjectRole
                WHERE ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.UserId=@UpdatedUserId AND EXISTS (
                    SELECT *
                    FROM ProjectMembers
                    WHERE ProjectMembers.UserId=@UserId AND ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.ProjectRole < @ProjectRole
                )";
                dbConnection.Open();

                dbConnection.Execute(sql, new {UserId = userId, UpdatedUserId = updatedUserId, ProjectId = projectId, ProjectRole = projectMember.ProjectRole});
            }
        }

        public void Delete(Guid projectMemberGuid, Guid projectGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                int userId = new UserRepository().GetId(email);
                int projectId = new ProjectRepository().GetId(projectGuid);

                string sql =
                @"DELETE FROM ProjectMembers
                WHERE ProjectMembers.ProjectMemberGUID=@ProjectMemberGUID AND EXISTS (
                    SELECT *
                    FROM ProjectMembers
                    WHERE ProjectMembers.UserId=@UserId AND ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.ProjectRole < @ProjectRole
                )";
                dbConnection.Open();

                dbConnection.Execute(sql, new {ProjectMemberGUID = projectMemberGuid, ProjectId = projectId, UserId = userId});
            }
        }
    }
}

/TODO/
projectmember role got from the api user cant be trusted (can modify others even if they have a higher role), instead get the role through queries