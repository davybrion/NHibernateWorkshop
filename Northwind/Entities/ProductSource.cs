using System;

namespace Northwind.Entities
{
    public class ProductSource : Entity<Guid>
    {
        public virtual Product Product { get; private set; }
        public virtual Supplier Supplier { get; private set; }
        public virtual double Cost { get; set; }

        protected ProductSource() {}

        public ProductSource(Product product, Supplier supplier, double cost)
        {
            Product = product;
            Supplier = supplier;
            Cost = cost;
        }
    }
}