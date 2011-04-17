using NHibernate;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.Criteria
{
    [TestFixture]
    public class Ordering : AutoRollbackFixture
    {
        [Test]
        public void order_by_own_property()
        {
            var customers = Session.CreateCriteria<Customer>()
                .AddOrder(new NHibernate.Criterion.Order("DiscountPercentage", true))
                .SetMaxResults(10)
                .List<Customer>();

            for (int i = 0; i < 9; i++)
            {
                Assert.LessOrEqual(customers[i].DiscountPercentage, customers[i+1].DiscountPercentage);
            }
        }

        [Test]
        public void order_by_joined_property()
        {
            var orders = Session.CreateCriteria<Order>()
                .CreateCriteria("Customer", "c")
                .AddOrder(new NHibernate.Criterion.Order("c.DiscountPercentage", false))
                .SetMaxResults(50)
                .List<Order>();

            for (int i = 0; i < 49; i++)
            {
                Assert.GreaterOrEqual(orders[i].Customer.DiscountPercentage, orders[i+1].Customer.DiscountPercentage);
            }
        }

        [Test]
        public void order_by_joined_property_fails_when_using_fetchmode()
        {
            var query = Session.CreateCriteria<Order>()
                .SetFetchMode("Customer", FetchMode.Join)
                .AddOrder(new NHibernate.Criterion.Order("Customer.DiscountPercentage", false))
                .SetMaxResults(10);

            Assert.Throws<NHibernate.QueryException>(() => query.List());
        }
    }
}