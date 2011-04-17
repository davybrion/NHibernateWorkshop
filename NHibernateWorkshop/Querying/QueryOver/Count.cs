using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.QueryOver
{
    [TestFixture]
    public class Count : AutoRollbackFixture
    {
        protected override void AfterSetUp()
        {
            Session.Save(new CustomerBuilder().WithDiscountPercentage(0.2d).Build());
        }

        [Test]
        public void rowcount_returns_size_of_resultset()
        {
            var query = Session.QueryOver<Customer>()
                .Where(c => c.DiscountPercentage == 0.2d);

            var resultSet = query.List();
            var count = query.RowCount();

            Assert.AreEqual(resultSet.Count, count);
        }
    }
}