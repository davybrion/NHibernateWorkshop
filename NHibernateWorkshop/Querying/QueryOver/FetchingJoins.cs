using System.Linq;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;
using Order = Northwind.Entities.Order;

namespace NHibernateWorkshop.Querying.QueryOver
{
    [TestFixture]
    public class FetchingJoins : AutoRollbackFixture
    {
        [Test]
        public void fetch_join_on_a_many_to_one_with_implicit_inner_join()
        {
            var ordersWithCustomers = Session.QueryOver<Order>()
                .JoinQueryOver(o => o.Customer) // implicit inner join because Customer is a required property
                .List();

            // IsInitialized would return false if Customer was a proxy
            ordersWithCustomers.Each(o => Assert.IsTrue(NHibernateUtil.IsInitialized(o.Customer)));
        }

        [Test]
        public void fetch_join_on_optional_many_to_one_does_not_do_implicit_outer_join()
        {
            var employeeWithoutManager = new EmployeeBuilder().Build();
            Session.Save(employeeWithoutManager);

            var employeeWithManager = new EmployeeBuilder().WithManager(employeeWithoutManager).Build();
            Session.Save(employeeWithManager);

            FlushAndClear();

            var employeesWithOrWithoutManagers = Session.QueryOver<Employee>()
                .JoinQueryOver(e => e.Manager)
                .List();

            var retrievedEmployeeWithoutManager = employeesWithOrWithoutManagers.FirstOrDefault(e => e.Id == employeeWithoutManager.Id);
            var retrievedEmployeeWithManager = employeesWithOrWithoutManagers.FirstOrDefault(e => e.Id == employeeWithManager.Id);
            Assert.IsNotNull(retrievedEmployeeWithManager);
            // retrievedEmployeeWithoutManager would not be null if an outer join was used
            Assert.IsNull(retrievedEmployeeWithoutManager);
            employeesWithOrWithoutManagers.Each(e => Assert.IsTrue(NHibernateUtil.IsInitialized(e.Manager)));
        }

        [Test]
        public void fetch_join_on_optional_many_to_one_with_explicit_outer_join()
        {
            var employeeWithoutManager = new EmployeeBuilder().Build();
            Session.Save(employeeWithoutManager);

            var employeeWithManager = new EmployeeBuilder().WithManager(employeeWithoutManager).Build();
            Session.Save(employeeWithManager);

            FlushAndClear();

            var employeesWithOrWithoutManagers = Session.QueryOver<Employee>()
                .JoinQueryOver(e => e.Manager, JoinType.LeftOuterJoin)
                .List();

            var retrievedEmployeeWithoutManager = employeesWithOrWithoutManagers.FirstOrDefault(e => e.Id == employeeWithoutManager.Id);
            var retrievedEmployeeWithManager = employeesWithOrWithoutManagers.FirstOrDefault(e => e.Id == employeeWithManager.Id);
            Assert.IsNotNull(retrievedEmployeeWithManager);
            Assert.IsNotNull(retrievedEmployeeWithoutManager);
            Assert.IsNull(retrievedEmployeeWithoutManager.Manager);
            Assert.IsTrue(NHibernateUtil.IsInitialized(retrievedEmployeeWithManager.Manager));
        }

        [Test]
        public void fetch_join_on_a_many_to_one_and_one_of_its_many_to_ones()
        {
            var ordersWithEmployeesAndTheirManagers = Session.QueryOver<Order>()
                .JoinQueryOver(o => o.Employee)
                .JoinQueryOver(e => e.Manager, JoinType.LeftOuterJoin) // if you want an outer join here, you have to be explicit
                .List();

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
            Employee employee = null;
            Customer customer = null;

            var ordersWithCustomersAndEmployees = Session.QueryOver<Order>()
                .JoinAlias(o => o.Employee, () => employee)
                .JoinAlias(o => o.Customer, () => customer)
                .List<Order>();

            ordersWithCustomersAndEmployees.Each(o => Assert.IsTrue(NHibernateUtil.IsInitialized(o.Employee)));
            ordersWithCustomersAndEmployees.Each(o => Assert.IsTrue(NHibernateUtil.IsInitialized(o.Customer)));
        }

        [Test]
        public void fetch_join_on_one_to_many_without_distinct_result_transformer()
        {
            var ordersWithItems = Session.QueryOver<Order>()
                .JoinQueryOver(o => o.Items)
                .List();

            var orderCount = Session.QueryOver<Order>().RowCount();
            var orderItemCount = Session.QueryOver<OrderItem>().RowCount();

            // oops... ordersWithItems contains an element for each OrderItem
            Assert.AreNotEqual(orderCount, ordersWithItems.Count);
            Assert.AreEqual(orderItemCount, ordersWithItems.Count);
        }

        [Test]
        public void fetch_join_on_one_to_many_with_distinct_result_transformer()
        {
            var ordersWithItems = Session.QueryOver<Order>()
                .JoinQueryOver(o => o.Items, JoinType.LeftOuterJoin)
                .TransformUsing(new DistinctRootEntityResultTransformer())
                .List();

            var orderCount = Session.QueryOver<Order>().RowCount();

            Assert.AreEqual(orderCount, ordersWithItems.Count);
            // note: using an inner join would make this assertion fail for some unclear reason... it would
            // also trigger a select when trying to access the Items collection
            ordersWithItems.Each(o => Assert.IsTrue(NHibernateUtil.IsInitialized(ReflectionHelper.GetPrivateFieldValue(o, "_items"))));
        }

        [Test]
        public void fetch_join_on_one_to_many_and_one_of_its_many_to_ones()
        {
            var ordersWithItemsAndProducts = Session.QueryOver<Order>()
                .JoinQueryOver(o => o.Items, JoinType.LeftOuterJoin)
                .JoinQueryOver(i => i.Product, JoinType.InnerJoin)
                .TransformUsing(new DistinctRootEntityResultTransformer())
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