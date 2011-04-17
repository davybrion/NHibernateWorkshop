using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;
using NHibernate.Linq;
using System.Linq;

namespace NHibernateWorkshop.Querying.LINQ
{
    [TestFixture]
    public class Count : AutoRollbackFixture
    {
        protected override void AfterSetUp()
        {
            Session.Save(new CustomerBuilder().WithDiscountPercentage(0.2d).Build());
        }

        [Test]
        public void count_returns_size_of_resultset()
        {
            var query = Session.Query<Customer>()
                .Where(c => c.DiscountPercentage == 0.2d);

            var resultSet = query.ToList();
            var count = query.Count();

            Assert.AreEqual(resultSet.Count, count);
        }
    }
}