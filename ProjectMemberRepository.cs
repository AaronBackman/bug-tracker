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

                if (userId == -1 || projectId == -1) {
                    return null;
                }

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

                if (newUserId == -1 || userId == -1 || projectId == -1) {
                    return null;
                }

                if (!isOwner) {
                    sql =
                    @"IF (EXISTS (
                        SELECT *
                        FROM ProjectMembers
                        WHERE ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.UserId=@UserId AND ProjectMembers.ProjectRole < @ProjectRole
                    ))
                    BEGIN
                        INSERT INTO ProjectMembers (ProjectRole, ProjectId, UserId)
                        OUTPUT INSERTED.ProjectMemberGUID
                        VALUES(@ProjectRole, @ProjectId, @NewUserId)
                    END";
                }
                else {
                    sql =
                    @"INSERT INTO ProjectMembers (ProjectRole, ProjectId, UserId)
                    OUTPUT INSERTED.ProjectMemberGUID
                    VALUES(@ProjectRole, @ProjectId, @NewUserId)
                    ";
                }

                try {
                    projectMember.ProjectMemberGUID = dbConnection.ExecuteScalar<Guid>(sql, new {ProjectRole = projectMember.ProjectRole, ProjectId = projectId, NewUserId = newUserId, UserId = userId});

                    return projectMember;
                }
                catch (SqlException) {
                    // todo return a boolean value to show the operation failed
                    return null;
                }
            }
        }

        public void Put(ProjectMember projectMember, Guid projectGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                
                int updatedUserId = new UserRepository().GetId(projectMember.Email);
                int userId = new UserRepository().GetId(email);
                int projectId = new ProjectRepository().GetId(projectGuid);
                Role? oldProjectRole = getMemberRoleByGUID(projectMember.ProjectMemberGUID);
                Role newProjectRole = projectMember.ProjectRole;

                if (updatedUserId == -1 || userId == -1 || projectId == -1 || oldProjectRole == null) {
                    return;
                }

                // cant change your own role
                if (updatedUserId == userId) {
                    return;
                }

                string sql =
                @"UPDATE ProjectMembers
                SET ProjectRole=@NewProjectRole
                WHERE ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.UserId=@UpdatedUserId AND EXISTS (
                    SELECT *
                    FROM ProjectMembers
                    WHERE ProjectMembers.UserId=@UserId AND ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.ProjectRole < @OldProjectRole AND ProjectMembers.ProjectRole < @NewProjectRole
                )";
                dbConnection.Open();

                dbConnection.Execute(sql, new {UserId = userId, UpdatedUserId = updatedUserId, ProjectId = projectId, OldProjectRole = oldProjectRole, NewProjectRole = newProjectRole});
            }
        }

        public void Delete(Guid projectMemberGuid, Guid projectGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                int userId = new UserRepository().GetId(email);
                int projectId = new ProjectRepository().GetId(projectGuid);
                Role? projectRole = getMemberRoleByGUID(projectMemberGuid);

                if (projectRole == null || userId == -1 || projectId == -1) {
                    return;
                }

                string sql =
                @"DELETE FROM ProjectMembers
                WHERE ProjectMembers.ProjectMemberGUID=@ProjectMemberGUID AND EXISTS (
                    SELECT *
                    FROM ProjectMembers
                    WHERE ProjectMembers.UserId=@UserId AND ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.ProjectRole < @ProjectRole
                )";
                dbConnection.Open();

                dbConnection.Execute(sql, new {ProjectMemberGUID = projectMemberGuid, ProjectId = projectId, UserId = userId, ProjectRole = projectRole});
            }
        }

        private Role? getMemberRoleByGUID(Guid projectMemberGuid) {
            using (IDbConnection dbConnection = Connection) {

                string sql =
                @"SELECT ProjectRole FROM ProjectMembers WHERE ProjectMembers.ProjectMemberGUID=@GUID";
                dbConnection.Open();

                var result = dbConnection.Query<Role>(sql, new {GUID = projectMemberGuid});

                if (result.Count() == 0) {
                    return null;
                }

                return result.First();;
            }
        }
    }
}