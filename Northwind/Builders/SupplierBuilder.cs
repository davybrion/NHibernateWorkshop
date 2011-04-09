using Northwind.Entities;

namespace Northwind.Builders
{
    public class SupplierBuilder : ThirdPartyBuilder<SupplierBuilder>
    {
        private string _website;

        public SupplierBuilder WithWebsite(string website)
        {
            _website = website;
            return this;
        }

        public Supplier Build()
        {
            var supplier = new Supplier(_name, _address) {Website = _website};
            SetInheritedOptionalProperties(supplier);
            return supplier;
        }
    }
}