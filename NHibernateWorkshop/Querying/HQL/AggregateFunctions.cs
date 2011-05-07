using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.HQL
{
    [TestFixture]
    public class AggregateFunctions : AutoRollbackFixture
    {
        private IList<Product> _products;

        protected override void AfterSetUp()
        {
            _products = Session.CreateQuery("from Product").List<Product>();
        }

        [Test]
        public void count()
        {
            var count = Session.CreateQuery("select count(p) from Product p where p.UnitsInStock > :minimum")
                .SetParameter("minimum", 10)
                .UniqueResult();

            Assert.AreEqual(_products.Count(p => p.UnitsInStock > 10), count);
        }

        [Test]
        public void sum()
        {
            var totalItemsInStock = Session.CreateQuery("select sum(p.UnitsInStock) from Product p").UniqueResult();
            Assert.AreEqual(_products.Sum(p => p.UnitsInStock), totalItemsInStock);
        }

        [Test]
        public void average()
        {
            var averageItemsInStock = Session.CreateQuery("select avg(p.UnitsInStock) from Product p").UniqueResult<double>();
#if HBMSQLSERVER || FLUENTSQLSERVER
            // SQL server rounds the avg value down if you don't cast the column to double precision... and HQL doesn't include the cast :s
            Assert.Less(Math.Abs(_products.Average(p => p.UnitsInStock).Value - averageItemsInStock), 1);
#else
            Assert.AreEqual(_products.Average(p => p.UnitsInStock), averageItemsInStock);
#endif
        }

        [Test]
        public void maximum()
        {
            var mostItemsInStock = Session.CreateQuery("select max(p.UnitsInStock) from Product p").UniqueResult();
            Assert.AreEqual(_products.Max(p => p.UnitsInStock), mostItemsInStock);
        }

        [Test]
        public void minimum()
        {
            var fewestItemsInStock = Session.CreateQuery("select min(p.UnitsInStock) from Product p").UniqueResult();
            Assert.AreEqual(_products.Min(p => p.UnitsInStock), fewestItemsInStock);
        }
    }
}