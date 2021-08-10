using System.Collections.Generic;

namespace bug_tracker
{
    public interface IUserRepository
    {
        public User GetByUsername(string username);
        public void Add(User user);

        public void Put(User user);
        public void Delete(User user);
    }
}