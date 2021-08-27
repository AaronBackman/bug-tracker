using Dapper;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace bug_tracker
{
    public class TicketRepository : ITicketRepository
    {
        private string connectionString;
        public IDbConnection Connection
        {
            get{
                return new SqlConnection(connectionString);
            }
        }

        public TicketRepository()
        {
            connectionString = "data source=carbon;initial catalog=BugTrackerDB;user id=BugTracker;password=Password123";
        }

        public List<Ticket> GetAll(Guid projectGuid, string email)
        {

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                int userId = new UserRepository().GetId(email);
                int projectId = new ProjectRepository().GetId(projectGuid);

                string sql = 
                    @"SELECT T.TicketName, T.TicketGUID, T.TicketDescription, T.DateCreated, T.TicketPriority, T.TicketCompleted, U.Nickname AS CreatorNickname
                    FROM Tickets T
                    INNER JOIN Users U ON Users.Id=Tickets.CreatorId
                    WHERE Tickets.ProjectId=@ProjectId
                    AND EXISTS
                        (SELECT *
                        FROM Users
                        INNER JOIN ProjectMembers
                        ON Users.Id=ProjectMembers.UserId
                        WHERE ProjectMembers.ProjectId=@ProjectId AND Users.Id=@Id
                        )";

                List<Ticket> tickets = dbConnection.Query<Ticket>(sql, new {ProjectId = projectId, Id = userId}).ToList<Ticket>();

                return tickets;
            }
        }

        public Ticket Add(Ticket ticket, Guid projectGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                dbConnection.Open();

                int userId = new UserRepository().GetId(email);
                int projectId = new ProjectRepository().GetId(projectGuid);
                string sql =
                    @"INSERT INTO Tickets (CreatorId, DateCreated, TicketName, TicketDescription, TicketPriority, TicketCompleted, ProjectId)
                    OUTPUT INSERTED.TicketGUID
                    VALUES(@UserId, @DateCreated, @TicketName, @TicketDescription, @TicketPriority, @TicketCompleted, @ProjectId)
                    SELECT *
                    FROM ProjectMembers
                    WHERE ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.UserId=@UserId AND ProjectMembers.ProjectRole <= @RequiredRole";

                ticket.TicketGUID = dbConnection.ExecuteScalar<Guid>(sql, new {
                        UserId = userId, DateCreated = ticket.DateCreated, TicketName = ticket.TicketName, TicketDescription = ticket.TicketDescription,
                        TicketPriority = ticket.TicketPriority, TicketCompleted = ticket.TicketCompleted, ProjectId = projectId, RequiredRole = Role.Developer
                    });

                return ticket;
            }
        }

        public void Put(Ticket ticket, Guid ticketGuid, Guid projectGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                
                int userId = new UserRepository().GetId(email);
                int projectId = new ProjectRepository().GetId(projectGuid);

                string sql =
                @"UPDATE Tickets
                SET TicketName=@Ticketname, TicketDescription=@TicketDescription, TicketPriority=@TicketPriority, TicketCompleted=@TicketCompleted
                WHERE Tickets.TicketGUID=@TicketGUID AND EXISTS (
                    SELECT *
                    FROM ProjectMembers
                    WHERE ProjectMembers.UserId=@UserId AND ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.ProjectRole <= @RequiredRole
                )";
                dbConnection.Open();

                dbConnection.Execute(sql, new {
                        UserId = userId, TicketName = ticket.TicketName, TicketDescription = ticket.TicketDescription,
                        TicketPriority = ticket.TicketPriority, TicketCompleted = ticket.TicketCompleted, ProjectId = projectId, RequiredRole = Role.Developer
                    });
            }
        }

        public void Delete(Guid ticketGuid, Guid projectGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                int userId = new UserRepository().GetId(email);
                int projectId = new ProjectRepository().GetId(projectGuid);

                string sql =
                @"DELETE FROM Tickets
                WHERE Tickets.TicketGUID=@TicketGUID AND EXISTS (
                    SELECT *
                    FROM ProjectMembers
                    WHERE ProjectMembers.UserId=@UserId AND ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.ProjectRole <= @RequiredRole
                )";
                dbConnection.Open();

                dbConnection.Execute(sql, new {
                        UserId = userId, ProjectId = projectId, RequiredRole = Role.Developer
                    });
            }
        }
    }
}