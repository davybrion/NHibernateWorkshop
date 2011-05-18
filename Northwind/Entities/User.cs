namespace Northwind.Entities
{
    public class User : Entity<int>
    {
        public virtual string UserName { get; set; }
        public virtual byte[] PasswordHash { get; set; }

        protected User() {}

        public User(string username, byte[] passwordHash)
        {
            UserName = username;
            PasswordHash = passwordHash;
        }
    }
}