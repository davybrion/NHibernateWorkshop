using Northwind.Builders;
using NUnit.Framework;

namespace NHibernateWorkshop.IdentifierStrategies
{
    [TestFixture]
    public class Identity : AutoRollbackFixture
    {
        [Test]
        public void attaching_a_transient_object_immediately_hits_the_database()
        {
            var employee = new EmployeeBuilder().Build();
            var insertCount = Statistics.EntityInsertCount;
            Assert.AreEqual(employee.Id, 0);
            Session.Save(employee);
            Assert.Greater(employee.Id, 0);
            // a new insert statement will have been issued already even though we haven't flushed the session
            Assert.AreEqual(insertCount + 1, Statistics.EntityInsertCount);
        }
    }
}