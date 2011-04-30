using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.Criteria
{
    [TestFixture]
    public class AggregateFunctions : AutoRollbackFixture
    {
        private IList<Product> _products;

        protected override void AfterSetUp()
        {
            _products = Session.CreateCriteria<Product>().List<Product>();
        }

        [Test]
        public void count()
        {
            var count = Session.CreateCriteria<Product>()
                .Add(Restrictions.Gt("UnitsInStock", 10))
                .SetProjection(Projections.RowCount())
                .UniqueResult();

            Assert.AreEqual(_products.Count(p => p.UnitsInStock > 10), count);
        }

        [Test]
        public void sum()
        {
            var totalItemsInStock = Session.CreateCriteria<Product>()
                .SetProjection(Projections.Sum("UnitsInStock"))
                .UniqueResult<int>();

            Assert.AreEqual(_products.Sum(p => p.UnitsInStock), totalItemsInStock);
        }

        [Test]
        public void average()
        {
            var averageItemsInStock = Session.CreateCriteria<Product>()
                .SetProjection(Projections.Avg("UnitsInStock"))
                .UniqueResult<double>();

            Assert.AreEqual(_products.Average(p => p.UnitsInStock), averageItemsInStock);
        }

        [Test]
        public void maximum()
        {
            var mostItemsInStock = Session.CreateCriteria<Product>()
                .SetProjection(Projections.Max("UnitsInStock"))
                .UniqueResult<int>();

            Assert.AreEqual(_products.Max(p => p.UnitsInStock), mostItemsInStock);
        }

        [Test]
        public void minimum()
        {
            var fewestItemsInStock = Session.CreateCriteria<Product>()
                .SetProjection(Projections.Min("UnitsInStock"))
                .UniqueResult<int>();

            Assert.AreEqual(_products.Min(p => p.UnitsInStock), fewestItemsInStock);
        }
    }
}