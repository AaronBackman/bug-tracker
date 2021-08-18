using Dapper;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace bug_tracker
{
    public class UserRepository : IUserRepository
    {
        private string connectionString;
        public IDbConnection Connection
        {
            get{
                return new SqlConnection(connectionString);
            }
        }

        public UserRepository()
        {
            connectionString = "data source=carbon;initial catalog=BugTrackerDB;user id=BugTracker;password=Password123";
        }

        public User GetByEmail(string email)
        {

            using (IDbConnection dbConnection = Connection)
            {
                string sql = @"SELECT * FROM Users WHERE Email=@Email";
                dbConnection.Open();

                User user = dbConnection.Query<User>(sql, new {Email = email}).FirstOrDefault();

                return user;
            }
        }

        public int GetId(string email)
        {
            using (IDbConnection dbConnection = Connection) {
                dbConnection.Open();

                string sql = @"SELECT Users.Id FROM Users WHERE Users.Email=@Email";
                int id = dbConnection.Query<int>(sql, new {Email = email}).FirstOrDefault();

                return id;
            }
        }

        public void Add(User user)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql = @"INSERT INTO Users (Email, Nickname) VALUES(@Email,@Nickname)";
                dbConnection.Open();

                dbConnection.Execute(sql, user);
            }
        }

        public void Put(User user)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql = @"UPDATE Users SET Nickname=@Nickname WHERE Email=@Email";
                dbConnection.Open();

                dbConnection.Execute(sql, user);
            }
        }

        public void Delete(User user)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql = @"DELETE FROM Users WHERE Email=@Email";
                dbConnection.Open();

                dbConnection.Execute(sql, user);
            }
        }
    }
}