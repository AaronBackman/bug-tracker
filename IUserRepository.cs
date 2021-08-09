using Dapper;
using System.Data;
using System.Collections.Generic;

namespace bug_tracker
{
    public interface IUserRepository
    {
        public IEnumerable<UserTest> GetByUsername(string username);
        public void Add(UserTest userTest);
    }
}