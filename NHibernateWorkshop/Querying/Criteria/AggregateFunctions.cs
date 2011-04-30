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
        [Test]
        public void count()
        {
            Session.Save(new CustomerBuilder().WithDiscountPercentage(0.2d).Build());
            FlushAndClear();

            var query = DetachedCriteria.For<Customer>()
                .Add(Restrictions.Gt("DiscountPercentage", 0.2d));

            var resultSet = query.GetExecutableCriteria(Session).List<Customer>();
            var count = query.SetProjection(Projections.RowCount()).GetExecutableCriteria(Session).UniqueResult();
            Assert.AreEqual(resultSet.Count, count);
        }

        [Test]
        public void sum()
        {
            var totalItemsInStock = Session.CreateCriteria<Product>()
                .SetProjection(Projections.Sum("UnitsInStock"))
                .UniqueResult<int>();

            var allProducts = Session.CreateCriteria<Product>().List<Product>();

            Assert.AreEqual(allProducts.Sum(p => p.UnitsInStock), totalItemsInStock);
        }

        [Test]
        public void average()
        {
            var averageItemsInStock = Session.CreateCriteria<Product>()
                .SetProjection(Projections.Avg("UnitsInStock"))
                .UniqueResult<double>();

            var allProducts = Session.CreateCriteria<Product>().List<Product>();

            Assert.AreEqual(allProducts.Average(p => p.UnitsInStock), averageItemsInStock);
        }

        [Test]
        public void maximum()
        {
            var mostItemsInStock = Session.CreateCriteria<Product>()
                .SetProjection(Projections.Max("UnitsInStock"))
                .UniqueResult<int>();

            var allProducts = Session.CreateCriteria<Product>().List<Product>();

            Assert.AreEqual(allProducts.Max(p => p.UnitsInStock), mostItemsInStock);
        }

        [Test]
        public void minimum()
        {
            var fewestItemsInStock = Session.CreateCriteria<Product>()
                .SetProjection(Projections.Min("UnitsInStock"))
                .UniqueResult<int>();

            var allProducts = Session.CreateCriteria<Product>().List<Product>();

            Assert.AreEqual(allProducts.Min(p => p.UnitsInStock), fewestItemsInStock);
        }
    }
}