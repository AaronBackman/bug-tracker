using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

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

        public IEnumerable<UserTest> GetByUsername(string username)
        {

            using (IDbConnection dbConnection = Connection)
            {
                string sql = @"SELECT * FROM Users WHERE Username=@Username";
                dbConnection.Open();

                return dbConnection.Query<UserTest>(sql, new {Username = username});
            }
        }

        public void Add(UserTest UserTest)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql = @"INSERT INTO Users (Username, Nickname) VALUES(@Username,@Nickname)";
                dbConnection.Open();

                dbConnection.Execute(sql, UserTest);
            }
        }
    }
}