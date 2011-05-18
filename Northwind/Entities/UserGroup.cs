namespace Northwind.Entities
{
    public class UserGroup : Entity<int>
    {
        public virtual string Name { get; set; }

        protected UserGroup() {}

        public UserGroup(string name)
        {
            Name = name;
        }
    }
}