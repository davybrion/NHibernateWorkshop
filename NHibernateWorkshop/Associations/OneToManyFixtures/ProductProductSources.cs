using System.Linq;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.AssociationFixtures.OneToManyFixtures
{
    // Product.Sources is the inverse end of a bidirectional relationship, with all-delete-orphan cascading
    [TestFixture]
    public class ProductProductSources : AutoRollbackFixture
    {
        private Product _product;
        private Supplier _supplier1;
        private Supplier _supplier2;
        private ProductSource _source1;
        private ProductSource _source2;

        protected override void AfterSetUp()
        {
            _product = new ProductBuilder().Build();
            _supplier1 = new SupplierBuilder().Build();
            _supplier2 = new SupplierBuilder().Build();
            _source1 = _product.AddSource(_supplier1, 10);
            _source2 = _product.AddSource(_supplier2, 12);
            Session.Save(_product);
            Flush();
        }

        [Test]
        public void save_cascades_to_sources()
        {
            Clear();
            var retrievedProduct = Session.Get<Product>(_product.Id);
            Assert.AreEqual(2, retrievedProduct.Sources.Count());
            // the calls to Contain only work because our entities have id-based equality (check the Equals impl)
            Assert.True(retrievedProduct.Sources.Contains(_source1));
            Assert.True(retrievedProduct.Sources.Contains(_source2));
        }

        [Test]
        public void removing_source_from_product_removes_source_from_database()
        {
            _product.RemoveSource(_supplier2);
            FlushAndClear();

            Assert.IsNull(Session.Get<ProductSource>(_source2.Id));
        }

        [Test]
        public void removing_product_from_database_removes_all_its_sources_from_database()
        {
            Session.Delete(_product);
            FlushAndClear();

            Assert.IsNull(Session.Get<Product>(_product.Id));
            Assert.IsNull(Session.Get<ProductSource>(_source1.Id));
            Assert.IsNull(Session.Get<ProductSource>(_source2.Id));
        }
    }
}