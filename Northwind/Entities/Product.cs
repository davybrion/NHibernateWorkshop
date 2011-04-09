using System.Collections.Generic;
using System.Linq;
using Northwind.Enums;

namespace Northwind.Entities
{
    public class Product : Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual ProductCategory Category { get; set; }
        public virtual double? UnitPrice { get; set; }
        public virtual int? UnitsInStock { get; set; }
        public virtual int? UnitsOnOrder { get; set; }
        public virtual int? ReorderLevel { get; set; }
        public virtual bool Discontinued { get; set; }
        public virtual int Version { get; set; }

        private IList<ProductSource> _sources = new List<ProductSource>();

        protected Product() {}

        public Product(string name, ProductCategory category)
        {
            Name = name;
            Category = category;
        }

        public virtual IEnumerable<ProductSource> Sources
        {
            // what would the consequence be of returning the result from .ToArray() here? (hint: this collection is marked as extra lazy)
            get { return _sources; }
        }

        public virtual ProductSource AddSource(Supplier supplier, double cost)
        {
            var productSource = new ProductSource(this, supplier, cost);
            _sources.Add(productSource);
            return productSource;
        }

        public virtual void RemoveSource(Supplier supplier)
        {
            _sources
                .Where(s => s.Supplier.Equals(supplier))
                .ToList()
                .ForEach(s => _sources.Remove(s));
        }
    }
}