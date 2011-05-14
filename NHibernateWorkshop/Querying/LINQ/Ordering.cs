using Northwind.Entities;
using NUnit.Framework;
using NHibernate.Linq;
using System.Linq;

namespace NHibernateWorkshop.Querying.LINQ
{
    [TestFixture]
    public class Ordering : AutoRollbackFixture
    {
        [Test]
        public void order_by_own_property()
        {
            var customers = Session.Query<Customer>()
                .OrderBy(c => c.DiscountPercentage)
                .Take(10)
                .ToList();

            for (int i = 0; i < 9; i++)
            {
                Assert.LessOrEqual(customers[i].DiscountPercentage, customers[i + 1].DiscountPercentage);
            }
        }

        [Test]
        public void order_by_joined_property()
        {
            var orders = Session.Query<Order>()
                .OrderByDescending(o => o.Customer.DiscountPercentage)
                .Take(50)
                .ToList();

            for (int i = 0; i < 49; i++)
            {
                Assert.GreaterOrEqual(orders[i].Customer.DiscountPercentage, orders[i + 1].Customer.DiscountPercentage);
            }
        }
    }
}