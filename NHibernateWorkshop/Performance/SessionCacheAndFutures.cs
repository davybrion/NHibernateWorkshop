using NHibernate.SqlCommand;
using NHibernate.Transform;
using Northwind.Entities;
using NUnit.Framework;
using System.Linq;
using NHibernate.Linq;

namespace NHibernateWorkshop.Performance
{
    [TestFixture]
    public class SessionCacheAndFutures : AutoRollbackFixture
    {
        [Test]
        public void large_query_with_joins_vs_batched_queries()
        {
            var fullOrderData = Session.CreateCriteria<Order>("order")
                .CreateCriteria("order.Customer", JoinType.InnerJoin)
                .CreateCriteria("order.Employee", JoinType.InnerJoin)
                .CreateCriteria("order.Items", "item", JoinType.InnerJoin)
                .CreateCriteria("item.Product", JoinType.InnerJoin)
                .SetResultTransformer(new DistinctRootEntityResultTransformer())
                .List<Order>();

            Clear();

            // the result of this query won't actually be used... the output is to demonstrate how awful it is
            // compared to the approach below

            var customers = Session.CreateCriteria<Customer>().Future<Customer>();
            var employees = Session.CreateCriteria<Employee>().Future<Employee>();
            var products = Session.CreateCriteria<Product>().Future<Product>();

            var ordersWithItems = Session.CreateCriteria<Order>()
                .CreateCriteria("Items", JoinType.LeftOuterJoin)
                .SetResultTransformer(new DistinctRootEntityResultTransformer())
                .Future<Order>();

            foreach (var orderWithItems in ordersWithItems)
            {
                Assert.AreSame(customers.First(c => c.Id == orderWithItems.Customer.Id), orderWithItems.Customer);
                Assert.AreSame(employees.First(e => e.Id == orderWithItems.Employee.Id), orderWithItems.Employee);

                foreach (var item in orderWithItems.Items)
                {
                    Assert.AreSame(products.First(p => p.Id == item.Product.Id), item.Product);
                }
            }
        }
    }
}