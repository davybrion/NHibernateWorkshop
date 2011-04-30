using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.LINQ
{
    [TestFixture]
    public class AggregateFunctions : AutoRollbackFixture
    {
        private IList<Product> _products;

        protected override void AfterSetUp()
        {
            _products = Session.Query<Product>().ToList();
        }

        [Test]
        public void count()
        {
            var count = Session.Query<Product>()
                .Count(p => p.UnitsInStock > 10);

            Assert.AreEqual(_products.Count(p => p.UnitsInStock > 10), count);
        }

        [Test]
        public void sum()
        {
            var totalItemsInStock = Session.Query<Product>().Sum(p => p.UnitsInStock);
            Assert.AreEqual(_products.Sum(p => p.UnitsInStock), totalItemsInStock);
        }

        [Test]
        public void average()
        {
            var averageItemsInStock = Session.Query<Product>().Average(p => p.UnitsInStock);
            Assert.AreEqual(_products.Average(p => p.UnitsInStock), averageItemsInStock);
        }

        [Test]
        public void maximum()
        {
            var mostItemsInStock = Session.Query<Product>().Max(p => p.UnitsInStock);
            Assert.AreEqual(_products.Max(p => p.UnitsInStock), mostItemsInStock);
        }

        [Test]
        public void minimum()
        {
            var fewestItemsInStock = Session.Query<Product>().Min(p => p.UnitsInStock);
            Assert.AreEqual(_products.Min(p => p.UnitsInStock), fewestItemsInStock);
        }
    }
}