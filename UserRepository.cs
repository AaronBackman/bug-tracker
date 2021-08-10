using Dapper;
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

        public User GetByUsername(string username)
        {

            using (IDbConnection dbConnection = Connection)
            {
                string sql = @"SELECT * FROM Users WHERE Username=@Username";
                dbConnection.Open();

                User user = dbConnection.Query<User>(sql, new {Username = username}).FirstOrDefault();
                if (user != null) {
                    string projectsSql = @"SELECT ProjectId FROM ProjectMembers INNER JOIN Users ON ProjectMembers.Username = Users.Username";
                    user.Projects = dbConnection.Query<int?>(projectsSql).ToList();
                }

                return user;
            }
        }

        public void Add(User user)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql = @"INSERT INTO Users (Username, Nickname) VALUES(@Username,@Nickname)";
                dbConnection.Open();

                dbConnection.Execute(sql, user);
            }
        }

        public void Put(User user)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql = @"UPDATE Users SET Nickname=@Nickname WHERE Username=@Username";
                dbConnection.Open();

                dbConnection.Execute(sql, user);
            }
        }

        public void Delete(User user)
        {
            using (IDbConnection dbConnection = Connection) {
                string sql = @"DELETE FROM Users WHERE Username=@Username";
                dbConnection.Open();

                dbConnection.Execute(sql, user);
            }
        }
    }
}