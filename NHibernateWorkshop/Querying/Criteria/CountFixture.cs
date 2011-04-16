using NHibernate.Criterion;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.Criteria
{
    [TestFixture]
    public class CountFixture : AutoRollbackFixture
    {
        [Test]
        public void setting_rowcount_projection_of_detached_criteria_returns_size_of_resultset()
        {
            var query = DetachedCriteria.For<Customer>()
                .Add(Restrictions.Gt("DiscountPercentage", 0.2d));

            var resultSet = query.GetExecutableCriteria(Session).List<Customer>();
            var count = query.SetProjection(Projections.RowCount()).GetExecutableCriteria(Session).UniqueResult();
            Assert.AreEqual(resultSet.Count, count);
        }

        [Test]
        public void setting_rowcount_of_criteria_returns_size_of_resultset()
        {
            var resultSet = Session.CreateCriteria<Customer>().List<Customer>();
            var count = Session.CreateCriteria<Customer>().SetProjection(Projections.RowCount()).UniqueResult();
            Assert.AreEqual(resultSet.Count, count);
        }
    }
}