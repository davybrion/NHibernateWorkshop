using Northwind.Builders;
using NUnit.Framework;

namespace NHibernateWorkshop.IdentifierStrategies
{
    [TestFixture]
    public class Hilo : AutoRollbackFixture
    {
        [Test]
        public void attaching_a_transient_object_does_not_hit_the_database_until_the_session_is_flushed()
        {
            var customer = new CustomerBuilder().Build();
            var insertCount = Statistics.EntityInsertCount;
            Assert.AreEqual(customer.Id, 0);
            Session.Save(customer);
            Assert.Greater(customer.Id, 0);
            // the ID has been filled in, but the insert hasn't been issued yet
            Assert.AreEqual(insertCount, Statistics.EntityInsertCount);
            // this will trigger the insert
            Session.Flush();
            Assert.AreEqual(insertCount + 1, Statistics.EntityInsertCount);
        }
    }
}