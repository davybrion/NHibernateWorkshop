using System;
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
            var averageItemsInStock = Session.Query<Product>().Average(p => p.UnitsInStock).Value;
#if HBMSQLSERVER || FLUENTSQLSERVER
            // SQL server rounds the avg value down if you don't cast the column to double precision... and the
            // generated query from the LINQ provider casts the result of the avg funtion to double precision, which
            // of course is too late (and kinda stupid)
            Assert.Less(Math.Abs(_products.Average(p => p.UnitsInStock).Value - averageItemsInStock), 1);
#else
            Assert.AreEqual(_products.Average(p => p.UnitsInStock), averageItemsInStock);
#endif
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