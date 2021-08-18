
namespace bug_tracker
{
    public interface IUserRepository
    {
        public User GetByEmail(string email);
        public void Add(User user);

        public void Put(User user);
        public void Delete(User user);

        public int GetId(string email);
    }
}