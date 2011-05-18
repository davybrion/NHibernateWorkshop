using Northwind.Entities;

namespace Northwind.Builders
{
    public class UserGroupBuilder
    {
        private string _name = "admins";

        public UserGroupBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public UserGroup Build()
        {
            return new UserGroup(_name);
        }
    }
}