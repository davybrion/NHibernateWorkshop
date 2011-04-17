using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.QueryOver
{
    [TestFixture]
    public class Ordering : AutoRollbackFixture
    {
        [Test]
        public void order_by_own_property()
        {
            var customers = Session.QueryOver<Customer>()
                .OrderBy(c => c.DiscountPercentage).Asc
                .Take(10)
                .List();

            for (int i = 0; i < 9; i++)
            {
                Assert.LessOrEqual(customers[i].DiscountPercentage, customers[i + 1].DiscountPercentage);
            }
        }

        [Test]
        public void order_by_joined_property()
        {
            var orders = Session.QueryOver<Order>()
                .JoinQueryOver(o => o.Customer)
                .OrderBy(c => c.DiscountPercentage).Desc
                .Take(50)
                .List();

            for (int i = 0; i < 49; i++)
            {
                Assert.GreaterOrEqual(orders[i].Customer.DiscountPercentage, orders[i + 1].Customer.DiscountPercentage);
            }
        }
    }
}