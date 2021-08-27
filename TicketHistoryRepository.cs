using Dapper;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace bug_tracker
{
    public class TicketHistoryRepository : ITicketHistoryRepository
    {
        private string connectionString;
        public IDbConnection Connection
        {
            get{
                return new SqlConnection(connectionString);
            }
        }

        public TicketHistoryRepository()
        {
            connectionString = "data source=carbon;initial catalog=BugTrackerDB;user id=BugTracker;password=Password123";
        }

        public List<TicketHistory> GetAll(Guid projectGuid, Guid ticketGuid, string email)
        {

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                int userId = new UserRepository().GetId(email);
                int projectId = new ProjectRepository().GetId(projectGuid);
                int ticketId = new TicketRepository().GetId(ticketGuid);

                string sql = 
                    @"SELECT TH.Change, TH.DateEdited, U.Nickname AS EditorNickname
                    FROM TicketHistories TH
                    INNER JOIN Users U ON Users.Id=TH.EditorId
                    WHERE TH.TicketId=@TicketId
                    AND EXISTS
                        (SELECT *
                        FROM Users
                        INNER JOIN ProjectMembers
                        ON Users.Id=ProjectMembers.UserId
                        WHERE ProjectMembers.ProjectId=@ProjectId AND Users.Id=@Id
                        )";

                List<TicketHistory> tickets = dbConnection.Query<TicketHistory>(sql, new {
                        TicketId = ticketId, ProjectId = projectId, Id = userId
                    }).ToList<TicketHistory>();

                return tickets;
            }
        }

        public void Add(TicketHistory ticketHistory, Guid projectGuid, Guid ticketGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                dbConnection.Open();

                int userId = new UserRepository().GetId(email);
                int projectId = new ProjectRepository().GetId(projectGuid);
                int ticketId = new TicketRepository().GetId(ticketGuid);
                string sql =
                    @"INSERT INTO TicketHistories (TicketId, DateEdited, EditorId, Change)
                    VALUES(@TicketId, @DateEdited, @EditorId, @Change)
                    SELECT *
                    FROM ProjectMembers
                    WHERE ProjectMembers.ProjectId=@ProjectId AND
                    ProjectMembers.UserId=@EditorId AND ProjectMembers.ProjectRole <= @RequiredRole";

                dbConnection.Execute(sql, new {
                        TicketId = ticketId, DateEdited = ticketHistory.DateEdited, EditorId = userId,
                        Change = ticketHistory.Change, ProjectId = projectId, RequiredRole = Role.Developer
                    });
            }
        }

        public void DeleteAll(Guid ticketGuid, Guid projectGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                int userId = new UserRepository().GetId(email);
                int projectId = new ProjectRepository().GetId(projectGuid);
                int ticketId = new TicketRepository().GetId(ticketGuid);

                string sql =
                @"DELETE FROM TicketHistories
                WHERE TicketHistories.TicketId=@TicketId AND EXISTS (
                    SELECT *
                    FROM ProjectMembers
                    WHERE ProjectMembers.UserId=@UserId AND ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.ProjectRole <= @RequiredRole
                )";
                dbConnection.Open();

                dbConnection.Execute(sql, new {
                        TicketId = ticketId, UserId = userId, ProjectId = projectId, RequiredRole = Role.Developer
                    });
            }
        }
    }
}