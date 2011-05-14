using System;
using Northwind.Builders;
using NUnit.Framework;

namespace NHibernateWorkshop.IdentifierStrategies
{
    [TestFixture]
    public class GuidComb : AutoRollbackFixture
    {
        [Test]
        public void attaching_a_transient_object_does_not_hit_the_database_until_the_session_is_flushed()
        {
            var order = new OrderBuilder().Build();
            Session.Save(order.Employee); // there's no cascade set on Order.Employee
            Session.Save(order.Customer); // saving the customer already as well, so we can flush these changes already
            Session.Flush();
            Assert.AreEqual(Guid.Empty, order.Id);
            var insertCount = Statistics.EntityInsertCount;
            Session.Save(order);
            Assert.AreNotEqual(Guid.Empty, order.Id);
            // the insert hasn't been issued yet
            Assert.AreEqual(insertCount, Statistics.EntityInsertCount);
            // this will trigger the insert
            Session.Flush();
            Assert.AreEqual(insertCount + 1, Statistics.EntityInsertCount);
        }
    }
}