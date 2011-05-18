namespace Northwind.Entities
{
    public class User : Entity<int>
    {
        public virtual string UserName { get; set; }
        public virtual byte[] PasswordHash { get; set; }
        public virtual Employee Employee { get; set; }

        protected User() {}

        public User(string username, byte[] passwordHash, Employee employee)
        {
            UserName = username;
            PasswordHash = passwordHash;
            Employee = employee;
        }
    }
}