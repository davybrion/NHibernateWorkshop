using Northwind.Components;

namespace Northwind.Entities
{
    public class Supplier : ThirdParty
    {
        public virtual string Website { get; set; }

        protected Supplier() {}

        public Supplier(string name, Address address) : base(name, address) {}
    }
}