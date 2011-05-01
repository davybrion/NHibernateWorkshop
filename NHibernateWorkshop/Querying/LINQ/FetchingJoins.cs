using NHibernate;
using NHibernate.Linq;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;
using System.Linq;
using Order = Northwind.Entities.Order;

namespace NHibernateWorkshop.Querying.LINQ
{
    [TestFixture]
    public class FetchingJoins : AutoRollbackFixture
    {
        [Test]
        public void fetch_join_on_a_many_to_one()
        {
            var ordersWithCustomers = Session.Query<Order>()
                .Fetch(o => o.Customer) // this will do an outer join by default... there's no way to specify join type
                .ToList();

            // IsInitialized would return false if Customer was a proxy
            ordersWithCustomers.Each(o => Assert.IsTrue(NHibernateUtil.IsInitialized(o.Customer)));
        }

        [Test]
        public void fetch_join_on_an_optional_many_to_one()
        {
            var employeeWithoutManager = new EmployeeBuilder().Build();
            Session.Save(employeeWithoutManager);

            var employeeWithManager = new EmployeeBuilder().WithManager(employeeWithoutManager).Build();
            Session.Save(employeeWithManager);

            FlushAndClear();

            var employeesWithOrWithoutManagers = Session.Query<Employee>()
                .Fetch(e => e.Manager) // again... outer join by default
                .ToList();

            var retrievedEmployeeWithoutManager = employeesWithOrWithoutManagers.FirstOrDefault(e => e.Id == employeeWithoutManager.Id);
            var retrievedEmployeeWithManager = employeesWithOrWithoutManagers.FirstOrDefault(e => e.Id == employeeWithManager.Id);
            Assert.IsNotNull(retrievedEmployeeWithManager);
            Assert.IsNotNull(retrievedEmployeeWithoutManager);
            Assert.IsNull(retrievedEmployeeWithoutManager.Manager);
            Assert.IsTrue(NHibernateUtil.IsInitialized(retrievedEmployeeWithManager.Manager));
        }

        [Test]
        public void fetch_join_on_an_optional_many_to_one_and_emulate_inner_join()
        {
            var employeeWithoutManager = new EmployeeBuilder().Build();
            Session.Save(employeeWithoutManager);

            var employeeWithManager = new EmployeeBuilder().WithManager(employeeWithoutManager).Build();
            Session.Save(employeeWithManager);

            FlushAndClear();

            var employeesWithManagers = Session.Query<Employee>()
                .Where(e => e.Manager != null)
                .Fetch(e => e.Manager)
                .ToList();

            var retrievedEmployeeWithoutManager = employeesWithManagers.FirstOrDefault(e => e.Id == employeeWithoutManager.Id);
            var retrievedEmployeeWithManager = employeesWithManagers.FirstOrDefault(e => e.Id == employeeWithManager.Id);
            Assert.IsNotNull(retrievedEmployeeWithManager);
            Assert.IsNull(retrievedEmployeeWithoutManager);
            employeesWithManagers.Each(e => Assert.IsTrue(NHibernateUtil.IsInitialized(e.Manager)));
        }

        [Test]
        public void fetch_join_on_a_many_to_one_and_one_of_its_many_to_ones()
        {
            var ordersWithEmployeesAndTheirManagers = Session.Query<Order>()
                .Fetch(o => o.Employee)
                .ThenFetch(e => e.Manager)
                .ToList();

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
        public void fetch_join_on_two_many_to_ones()
        {
            var ordersWithCustomersAndEmployees = Session.Query<Order>()
                .Fetch(o => o.Customer)
                .Fetch(o => o.Employee)
                .ToList();

            ordersWithCustomersAndEmployees.Each(o => Assert.IsTrue(NHibernateUtil.IsInitialized(o.Employee)));
            ordersWithCustomersAndEmployees.Each(o => Assert.IsTrue(NHibernateUtil.IsInitialized(o.Customer)));
        }

        [Test]
        public void fetch_join_on_one_to_many_does_not_require_result_transformer()
        {
            var ordersWithItems = Session.Query<Order>()
                .FetchMany(o => o.Items)
                .ToList();

            var orderCount = Session.Query<Order>().Count();

            Assert.AreEqual(orderCount, ordersWithItems.Count);
        }

        [Test]
        public void fetch_join_on_one_to_many_and_one_of_its_many_to_ones()
        {
            var ordersWithItemsAndProducts = Session.Query<Order>()
                .FetchMany(o => o.Items)
                .ThenFetch(i => i.Product)
                .ToList();

            ordersWithItemsAndProducts.Each(o =>
            {
                Assert.IsTrue(NHibernateUtil.IsInitialized(ReflectionHelper.GetPrivateFieldValue(o, "_items")));
                o.Items.Each(i => Assert.IsTrue(NHibernateUtil.IsInitialized(i.Product)));
            });
        }
    }
}