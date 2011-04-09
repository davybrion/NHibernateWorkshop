using Northwind.Entities;

namespace Northwind.Builders
{
    public class ProductSourceBuilder
    {
        private Product _product = new ProductBuilder().Build();
        private Supplier _supplier = new SupplierBuilder().Build();
        private double _cost = 15.6d;

        public ProductSourceBuilder WithProduct(Product product)
        {
            _product = product;
            return this;
        }

        public ProductSourceBuilder WithSupplier(Supplier supplier)
        {
            _supplier = supplier;
            return this;
        }

        public ProductSourceBuilder WithCost(double cost)
        {
            _cost = cost;
            return this;
        }

        public ProductSource Build()
        {
            return new ProductSource(_product, _supplier, _cost);
        }
    }
}