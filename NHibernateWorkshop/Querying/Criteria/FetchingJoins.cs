using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;
using Order = Northwind.Entities.Order;

namespace NHibernateWorkshop.Querying.Criteria
{
    [TestFixture]
    public class FetchingJoins : AutoRollbackFixture
    {
        [Test]
        public void fetch_join_on_a_many_to_one_with_implicit_inner_join()
        {
            var ordersWithCustomers = Session.CreateCriteria<Order>()
                .SetFetchMode("Customer", FetchMode.Join) // implicit inner join because Customer is a required property
                .List<Order>();

            // IsInitialized would return false if Customer was a proxy
            ordersWithCustomers.Each(o => Assert.IsTrue(NHibernateUtil.IsInitialized(o.Customer)));
        }

        [Test]
        public void fetch_join_on_a_many_to_one_with_implicit_outer_join()
        {
            var employeeWithoutManager = new EmployeeBuilder().Build();
            Session.Save(employeeWithoutManager);

            var employeeWithManager = new EmployeeBuilder().WithManager(employeeWithoutManager).Build();
            Session.Save(employeeWithManager);

            FlushAndClear();

            var employeesWithOrWithoutManagers = Session.CreateCriteria<Employee>()
                .SetFetchMode("Manager", FetchMode.Join) // implicit outer join because Manager is an optional property
                .List<Employee>();

            var retrievedEmployeeWithoutManager = employeesWithOrWithoutManagers.FirstOrDefault(e => e.Id == employeeWithoutManager.Id);
            var retrievedEmployeeWithManager = employeesWithOrWithoutManagers.FirstOrDefault(e => e.Id == employeeWithManager.Id);
            Assert.IsNotNull(retrievedEmployeeWithManager);
            Assert.IsNotNull(retrievedEmployeeWithoutManager);
            Assert.IsNull(retrievedEmployeeWithoutManager.Manager);
            Assert.IsTrue(NHibernateUtil.IsInitialized(retrievedEmployeeWithManager.Manager));
        }

        [Test]
        public void fetch_join_on_a_many_to_one_with_explicit_inner_join()
        {
            var employeeWithoutManager = new EmployeeBuilder().Build();
            Session.Save(employeeWithoutManager);

            var employeeWithManager = new EmployeeBuilder().WithManager(employeeWithoutManager).Build();
            Session.Save(employeeWithManager);

            FlushAndClear();

            var employeesWithManagers = Session.CreateCriteria<Employee>()
                .CreateCriteria("Manager", JoinType.InnerJoin)
                .List<Employee>();

            var retrievedEmployeeWithoutManager = employeesWithManagers.FirstOrDefault(e => e.Id == employeeWithoutManager.Id);
            var retrievedEmployeeWithManager = employeesWithManagers.FirstOrDefault(e => e.Id == employeeWithManager.Id);
            Assert.IsNotNull(retrievedEmployeeWithManager);
            Assert.IsNull(retrievedEmployeeWithoutManager);
            employeesWithManagers.Each(e => Assert.IsTrue(NHibernateUtil.IsInitialized(e.Manager)));
        }

        [Test]
        public void fetch_join_on_a_many_to_one_and_one_of_its_many_to_ones()
        {
            var ordersWithEmployeesAndTheirManagers = Session.CreateCriteria<Order>()
                .CreateCriteria("Employee", "e")
                .CreateCriteria("e.Manager", JoinType.LeftOuterJoin) // if you want an outer join here, you have to be explicit
                .List<Order>();

            ordersWithEmployeesAndTheirManagers.Each(o =>
            {
                Assert.IsTrue(NHibernateUtil.IsInitialized(o.Employee));

                if (o.Employee.Manager != null)
                {
                    Assert.IsTrue(NHibernateUtil.IsInitialized(o.Employee.Manager));
                }    
            });
        }

        [Test]
        public void fetch_join_on_two_many_to_ones_with_implicit_inner_joins()
        {
            var ordersWithCustomersAndEmployees = Session.CreateCriteria<Order>()
                .SetFetchMode("Employee", FetchMode.Join)
                .SetFetchMode("Customer", FetchMode.Join)
                .List<Order>();

            ordersWithCustomersAndEmployees.Each(o => Assert.IsTrue(NHibernateUtil.IsInitialized(o.Employee)));
            ordersWithCustomersAndEmployees.Each(o => Assert.IsTrue(NHibernateUtil.IsInitialized(o.Customer)));
        }

        [Test]
        public void fetch_join_on_one_to_many_without_distinct_result_transformer()
        {
            var ordersWithItems = Session.CreateCriteria<Order>()
                .SetFetchMode("Items", FetchMode.Join)
                .List<Order>();

            var orderCount = Session.CreateCriteria<Order>()
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();

            var orderItemCount = Session.CreateCriteria<OrderItem>()
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();

            // oops... ordersWithItems contains an element for each OrderItem
            Assert.AreNotEqual(orderCount, ordersWithItems.Count);
            Assert.AreEqual(orderItemCount, ordersWithItems.Count);
        }

        [Test]
        public void fetch_join_on_one_to_many_with_distinct_result_transformer()
        {
            var ordersWithItems = Session.CreateCriteria<Order>()
                .SetFetchMode("Items", FetchMode.Join)
                .SetResultTransformer(new DistinctRootEntityResultTransformer())
                .List<Order>();

            var orderCount = Session.CreateCriteria<Order>()
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();

            Assert.AreEqual(orderCount, ordersWithItems.Count);
            ordersWithItems.Each(o => Assert.IsTrue(NHibernateUtil.IsInitialized(ReflectionHelper.GetPrivateFieldValue(o, "_items"))));
        }

        [Test]
        public void fetch_join_on_one_to_many_and_one_of_its_many_to_ones()
        {
            var ordersWithItemsAndProducts = Session.CreateCriteria<Order>()
                .CreateCriteria("Items", "items", JoinType.LeftOuterJoin) 
                .CreateCriteria("items.Product", JoinType.InnerJoin)
                .SetResultTransformer(new DistinctRootEntityResultTransformer())
                .List<Order>();

            ordersWithItemsAndProducts.Each(o =>
            {
                // note: using an inner join would make this assertion fail for some unclear reason... it would
                // also trigger a select when trying to access the Items collection
                Assert.IsTrue(NHibernateUtil.IsInitialized(ReflectionHelper.GetPrivateFieldValue(o, "_items")));
                o.Items.Each(i => Assert.IsTrue(NHibernateUtil.IsInitialized(i.Product)));
            });
        }
    }
}