using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;

namespace Northwind.Entities
{
    public class UserGroup : Entity<int>
    {
        public virtual string Name { get; set; }

        private ISet<User> _users = new HashedSet<User>();

        protected UserGroup() {}

        public UserGroup(string name)
        {
            Name = name;
        }

        public virtual IEnumerable<User> Users
        {
            get { return _users.ToArray(); }
        }

        public virtual void AddUser(User user)
        {
            _users.Add(user);
            if (!user.UserGroups.Contains(this))
            {
                user.AddUserGroup(this);
            }
        }

        public virtual void RemoveUser(User user)
        {
            _users.Remove(user);
            if (user.UserGroups.Contains(this))
            {
                user.RemoveUserGroup(this);
            }
        }
    }
}