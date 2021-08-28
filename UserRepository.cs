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

                var result = dbConnection.Query<User>(sql, new {Email = email});

                if (result.Count() == 0) {
                    return null;
                }

                return result.First();;
            }
        }

        public int GetId(string email)
        {
            using (IDbConnection dbConnection = Connection) {
                dbConnection.Open();

                string sql = @"SELECT Users.Id FROM Users WHERE Users.Email=@Email";
                var result = dbConnection.Query<int>(sql, new {Email = email});

                if (result.Count() == 0) {
                    return -1;
                }

                return result.First();;
            }
        }

        public void Add(User user)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql = @"INSERT INTO Users (Email, Nickname) VALUES(@Email,@Nickname)";
                dbConnection.Open();

                try {
                    dbConnection.Execute(sql, user);
                }
                // happens for example if duplicate emails are inserted
                catch (SqlException) {
                    // todo return a boolean value to show the operation failed
                }
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

        public void Delete(string email)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql = @"DELETE FROM Users WHERE Email=@Email";
                dbConnection.Open();

                dbConnection.Execute(sql, new {Email = email});
            }
        }
    }
}