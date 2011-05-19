using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;

namespace Northwind.Entities
{
    public class User : Entity<int>
    {
        public virtual string UserName { get; set; }
        public virtual byte[] PasswordHash { get; set; }
        public virtual Employee Employee { get; set; }

        private ISet<UserGroup> _userGroups = new HashedSet<UserGroup>();

        protected User() {}

        public User(string username, byte[] passwordHash, Employee employee)
        {
            UserName = username;
            PasswordHash = passwordHash;
            Employee = employee;
        }

        public virtual IEnumerable<UserGroup> UserGroups
        {
            get { return _userGroups.ToArray(); }
        }

        public virtual void AddUserGroup(UserGroup userGroup)
        {
            _userGroups.Add(userGroup);
            if (!userGroup.Users.Contains(this))
            {
                userGroup.AddUser(this);
            }
        }

        public virtual void RemoveUserGroup(UserGroup userGroup)
        {
            _userGroups.Remove(userGroup);
            if (userGroup.Users.Contains(this))
            {
                userGroup.RemoveUser(this);
            }
        }
    }
}