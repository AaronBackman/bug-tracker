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

                if (userId == -1 || projectId == -1) {
                    return null;
                }

                string sql = 
                    @"SELECT T.TicketName, T.TicketGUID, T.TicketDescription, T.DateCreated, T.TicketPriority, T.TicketCompleted, U.Nickname AS CreatorNickname, A.Email AS AssignedToEmail
                    FROM Tickets T
                    INNER JOIN Users U ON U.Id=T.CreatorId
                    INNER JOIN Users A ON A.Id=T.AssignedToId
                    WHERE T.ProjectId=@ProjectId
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
                int assignedToId = new UserRepository().GetId(ticket.AssignedToEmail);
                int projectId = new ProjectRepository().GetId(projectGuid);

                if (userId == -1 || projectId == -1) {
                    return null;
                }

                string sql =
                    @"
                    IF EXISTS (
                        SELECT *
                        FROM ProjectMembers
                        WHERE ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.UserId=@UserId AND ProjectMembers.ProjectRole <= @RequiredRole
                    )
                    BEGIN
                        INSERT INTO Tickets (CreatorId, DateCreated, TicketName, TicketDescription, TicketPriority, TicketCompleted, ProjectId, AssignedToId)
                        OUTPUT INSERTED.TicketGUID
                        VALUES(@UserId, @DateCreated, @TicketName, @TicketDescription, @TicketPriority, @TicketCompleted, @ProjectId, @AssignedToId)
                    END";

                ticket.TicketGUID = dbConnection.ExecuteScalar<Guid>(sql, new {
                        UserId = userId, DateCreated = ticket.DateCreated, TicketName = ticket.TicketName, TicketDescription = ticket.TicketDescription,
                        TicketPriority = ticket.TicketPriority, TicketCompleted = ticket.TicketCompleted, ProjectId = projectId, RequiredRole = Role.Developer,
                        AssignedToId = assignedToId
                    });

                return ticket;
            }
        }

        public void Put(Ticket ticket, Guid ticketGuid, Guid projectGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                
                int userId = new UserRepository().GetId(email);
                int assignedToId = new UserRepository().GetId(ticket.AssignedToEmail);
                int projectId = new ProjectRepository().GetId(projectGuid);
                int ticketId = GetId(ticketGuid);

                if (userId == -1 || projectId == -1 || ticketId == -1) {
                    Console.WriteLine("Abort");
                    return;
                }

                string sql =
                @"UPDATE Tickets
                SET TicketName=@TicketName, TicketDescription=@TicketDescription, TicketPriority=@TicketPriority, TicketCompleted=@TicketCompleted, AssignedToId=@AssignedToId
                WHERE Tickets.Id=@TicketId AND EXISTS (
                    SELECT *
                    FROM ProjectMembers
                    WHERE ProjectMembers.UserId=@UserId AND ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.ProjectRole <= @RequiredRole
                )";
                dbConnection.Open();

                dbConnection.Execute(sql, new {
                        UserId = userId, TicketId = ticketId, TicketName = ticket.TicketName, TicketDescription = ticket.TicketDescription,
                        TicketPriority = ticket.TicketPriority, TicketCompleted = ticket.TicketCompleted, ProjectId = projectId, RequiredRole = Role.Developer,
                        AssignedToId = assignedToId
                    });
            }
        }

        public void Delete(Guid ticketGuid, Guid projectGuid, string email)
        {
            using (IDbConnection dbConnection = Connection) {
                int userId = new UserRepository().GetId(email);
                int projectId = new ProjectRepository().GetId(projectGuid);
                int ticketId = GetId(ticketGuid);

                if (userId == -1 || projectId == -1 || ticketId == -1) {
                    return;
                }

                string sql =
                @"DELETE FROM Tickets
                WHERE Tickets.Id=@TicketId AND EXISTS (
                    SELECT *
                    FROM ProjectMembers
                    WHERE ProjectMembers.UserId=@UserId AND ProjectMembers.ProjectId=@ProjectId AND ProjectMembers.ProjectRole <= @RequiredRole
                )";
                dbConnection.Open();

                dbConnection.Execute(sql, new {
                        UserId = userId, ProjectId = projectId, TicketId = ticketId, RequiredRole = Role.Developer
                    });
            }
        }

        public int GetId(Guid guid)
        {
            using (IDbConnection dbConnection = Connection) {
                dbConnection.Open();

                string sql = @"SELECT Tickets.Id FROM Tickets WHERE Tickets.TicketGUID=@Guid";
                var result = dbConnection.Query<int>(sql, new {Guid = guid});

                if (result.Count() == 0) {
                    return -1;
                }

                return result.First();;
            }
        }
    }
}