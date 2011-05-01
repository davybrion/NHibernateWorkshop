using Northwind.Components;

namespace Northwind.Entities
{
    public abstract class ThirdParty : Entity<int> 
    {
        protected ThirdParty() {}

        protected ThirdParty(string name, Address address)
        {
            Name = name;
            Address = address;
        }

        public virtual string Name { get; set; }
        public virtual Address Address { get; set; }
        public virtual string ContactName { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Fax { get; set; }
    }
}