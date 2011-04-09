using Northwind.Components;

namespace Northwind.Entities
{
    public class Customer : ThirdParty
    {
        public virtual double DiscountPercentage { get; set; }

        protected Customer() {}

        public Customer(string name, Address address) : base(name, address)
        {
            DiscountPercentage = 0;
        }
    }
}