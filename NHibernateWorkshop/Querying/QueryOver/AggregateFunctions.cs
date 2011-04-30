using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.QueryOver
{
    [TestFixture]
    public class AggregateFunctions : AutoRollbackFixture
    {
        private IList<Product> _products;

        protected override void AfterSetUp()
        {
            _products = Session.QueryOver<Product>().List<Product>();
        }

        [Test]
        public void count()
        {
            var count = Session.QueryOver<Product>()
                .Where(p => p.UnitsInStock > 10)
                .RowCount();

            Assert.AreEqual(_products.Count(p => p.UnitsInStock > 10), count);
        }

        [Test]
        public void sum()
        {
            var totalItemsInStock = Session.QueryOver<Product>()
                .Select(Projections.Sum<Product>(p => p.UnitsInStock))
                .SingleOrDefault<int>();

            Assert.AreEqual(_products.Sum(p => p.UnitsInStock), totalItemsInStock);
        }

        [Test]
        public void average()
        {
            var averageItemsInStock = Session.QueryOver<Product>()
                .Select(Projections.Avg<Product>(p => p.UnitsInStock))
                .SingleOrDefault<double>();

            Assert.AreEqual(_products.Average(p => p.UnitsInStock), averageItemsInStock);
        }

        [Test]
        public void maximum()
        {
            var mostItemsInStock = Session.QueryOver<Product>()
                .Select(Projections.Max<Product>(p => p.UnitsInStock))
                .SingleOrDefault<int>();

            Assert.AreEqual(_products.Max(p => p.UnitsInStock), mostItemsInStock);
        }

        [Test]
        public void minimum()
        {
            var fewestItemsInStock = Session.QueryOver<Product>()
                .Select(Projections.Min<Product>(p => p.UnitsInStock))
                .SingleOrDefault<int>();

            Assert.AreEqual(_products.Min(p => p.UnitsInStock), fewestItemsInStock);
        }
    }
}