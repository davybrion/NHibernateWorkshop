using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.HQL
{
    [TestFixture]
    public class Ordering : AutoRollbackFixture
    {
        [Test]
        public void order_by_own_property()
        {
            var customers = Session.CreateQuery("from Customer c order by c.DiscountPercentage asc")
                .SetMaxResults(10)
                .List<Customer>();

            for (int i = 0; i < 9; i++)
            {
                Assert.LessOrEqual(customers[i].DiscountPercentage, customers[i + 1].DiscountPercentage);
            }
        }

        [Test]
        public void order_by_joined_property()
        {
            var orders = Session.CreateQuery("from Order o inner join fetch o.Customer as customer order by customer.DiscountPercentage desc")
                    .SetMaxResults(50)
                    .List<Order>();

            for (int i = 0; i < 49; i++)
            {
                Assert.GreaterOrEqual(orders[i].Customer.DiscountPercentage, orders[i + 1].Customer.DiscountPercentage);
            }
        }
    }
}