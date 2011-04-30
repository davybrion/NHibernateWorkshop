using System;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.OptimisticConcurrency
{
    [TestFixture]
    public class OptimisticLockAll : Fixture
    {
        private Order _order;

        [SetUp]
        public void SetUp()
        {
            _order = new OrderBuilder().Build();
        }

        [TearDown]
        public void TearDown()
        {
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                session
                    .CreateQuery("delete from Northwind.Entities.Order o where o.Id = :id")
                    .SetParameter("id", _order.Id)
                    .ExecuteUpdate();

                transaction.Commit();
            }
        }

        [Test]
        public void simultaneous_changes_that_dont_conflict_are_not_possible()
        {
            using (var session1 = CreateSession())
            using (var session2 = CreateSession())
            {
                using (var trans1sess1 = session1.BeginTransaction())
                {
                    session1.Save(_order.Employee);
                    session1.Save(_order);
                    trans1sess1.Commit();
                }

                using (var trans1sess2 = session2.BeginTransaction())
                {
                    var theOrder = session2.Get<Order>(_order.Id);
                    theOrder.OrderedOn = DateTime.Now.AddDays(-1);
                    trans1sess2.Commit();
                }

                using (var trans2sess1 = session1.BeginTransaction())
                {
                    _order.ShippedOn = DateTime.Now;
                    Assert.Throws<NHibernate.StaleObjectStateException>(() => trans2sess1.Commit());
                    trans2sess1.Rollback();
                }
            }
        }
    }
}