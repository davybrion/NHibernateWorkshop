using Northwind.Entities;

namespace Northwind.Builders
{
    public class CustomerBuilder : ThirdPartyBuilder<CustomerBuilder>
    {
        private double _discountPercentage = 2;

        public CustomerBuilder WithDiscountPercentage(double discountPercentage)
        {
            _discountPercentage = discountPercentage;
            return this;
        }

        public Customer Build()
        {
            var customer = new Customer(_name, _address) {DiscountPercentage = _discountPercentage};
            SetInheritedOptionalProperties(customer);
            return customer;
        }
    }
}