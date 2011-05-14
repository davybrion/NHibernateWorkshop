using System;
using Northwind.Builders;
using NUnit.Framework;

namespace NHibernateWorkshop.IdentifierStrategies
{
    [TestFixture]
    public class AssignedGuid : AutoRollbackFixture
    {
        [Test]
        public void attaching_a_transient_object_does_not_hit_the_database_until_the_session_is_flushed()
        {
            var product = new ProductBuilder().Build();
            // of course, it already has an ID value since it's been assigned in the Product constructor
            Assert.AreNotEqual(Guid.Empty, product.Id);
            var insertCount = Statistics.EntityInsertCount;
            Session.Save(product);
            // the insert hasn't been issued yet
            Assert.AreEqual(insertCount, Statistics.EntityInsertCount);
            // this will trigger the insert
            Session.Flush();
            Assert.AreEqual(insertCount + 1, Statistics.EntityInsertCount);
        }
    }
}