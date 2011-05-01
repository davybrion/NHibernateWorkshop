using NHibernate;
using NHibernate.Transform;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;
using System.Linq;
using Order = Northwind.Entities.Order;

namespace NHibernateWorkshop.Querying.HQL
{
    [TestFixture]
    public class FetchingJoins : AutoRollbackFixture
    {
        [Test]
        public void fetch_join_on_a_many_to_one_with_explicit_fetch()
        {
            var ordersWithCustomers = Session.CreateQuery("from Order o join fetch o.Customer").List<Order>();
            // IsInitialized would return false if Customer was a proxy
            ordersWithCustomers.Each(o => Assert.IsTrue(NHibernateUtil.IsInitialized(o.Customer)));
        }

        [Test]
        public void fetch_join_on_a_many_to_one_without_explicit_fetch_and_without_select()
        {
            // if you don't specify a fetch or a select, you can only retrieve an object array
            var ordersWithCustomers = Session.CreateQuery("from Order o join o.Customer").List<object[]>();

            ordersWithCustomers.Each(a =>
            {
                Assert.IsInstanceOf<Order>(a[0]);
                Assert.IsInstanceOf<Customer>(a[1]);
                Assert.AreEqual(((Order)a[0]).Customer, a[1]);
                Assert.IsTrue(NHibernateUtil.IsInitialized(((Order)a[0]).Customer));
            });
        }

        [Test]
        public void fetch_join_on_a_many_to_one_without_explicit_fetch_and_with_select()
        {
            var ordersWithCustomers = Session.CreateQuery("select o from Order o join o.Customer").List<Order>();
            // despite the join, the order instances don't have their customer initialized yet and accessing the customer would
            // cause a select to retrieve the customer
            ordersWithCustomers.Each(o => Assert.IsFalse(NHibernateUtil.IsInitialized(o.Customer)));
        }

        [Test]
        public void fetch_outer_join_on_an_optional_many_to_one()
        {
            var employeeWithoutManager = new EmployeeBuilder().Build();
            Session.Save(employeeWithoutManager);

            var employeeWithManager = new EmployeeBuilder().WithManager(employeeWithoutManager).Build();
            Session.Save(employeeWithManager);

            FlushAndClear();

            var employeesWithOrWithoutManagers =
                Session.CreateQuery("from Employee e left join fetch e.Manager").List<Employee>();

            var retrievedEmployeeWithoutManager = employeesWithOrWithoutManagers.FirstOrDefault(e => e.Id == employeeWithoutManager.Id);
            var retrievedEmployeeWithManager = employeesWithOrWithoutManagers.FirstOrDefault(e => e.Id == employeeWithManager.Id);
            Assert.IsNotNull(retrievedEmployeeWithManager);
            Assert.IsNotNull(retrievedEmployeeWithoutManager);
            Assert.IsNull(retrievedEmployeeWithoutManager.Manager);
            Assert.IsTrue(NHibernateUtil.IsInitialized(retrievedEmployeeWithManager.Manager));
        }

        [Test]
        public void fetch_inner_join_on_an_optional_many_to_one()
        {
            var employeeWithoutManager = new EmployeeBuilder().Build();
            Session.Save(employeeWithoutManager);

            var employeeWithManager = new EmployeeBuilder().WithManager(employeeWithoutManager).Build();
            Session.Save(employeeWithManager);

            FlushAndClear();

            var employeesWithManagers = Session.CreateQuery("from Employee e join fetch e.Manager").List<Employee>();

            var retrievedEmployeeWithoutManager = employeesWithManagers.FirstOrDefault(e => e.Id == employeeWithoutManager.Id);
            var retrievedEmployeeWithManager = employeesWithManagers.FirstOrDefault(e => e.Id == employeeWithManager.Id);
            Assert.IsNotNull(retrievedEmployeeWithManager);
            Assert.IsNull(retrievedEmployeeWithoutManager);
            employeesWithManagers.Each(e => Assert.IsTrue(NHibernateUtil.IsInitialized(e.Manager)));
        }

        [Test]
        public void fetch_join_on_a_many_to_one_and_one_of_its_many_to_ones()
        {
            var ordersWithEmployeesAndTheirManagers =
                Session.CreateQuery("from Order o join fetch o.Employee e left join fetch e.Manager").List<Order>();

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
            var ordersWithCustomersAndEmployees =
                Session.CreateQuery("from Order o join fetch o.Customer join fetch o.Employee").List<Order>();

            ordersWithCustomersAndEmployees.Each(o => Assert.IsTrue(NHibernateUtil.IsInitialized(o.Employee)));
            ordersWithCustomersAndEmployees.Each(o => Assert.IsTrue(NHibernateUtil.IsInitialized(o.Customer)));
        }

        [Test]
        public void fetch_join_on_one_to_many_without_distinct_result_transformer()
        {
            var ordersWithItems = Session.CreateQuery("from Order o join fetch o.Items").List<Order>();
            var orderCount = Session.CreateQuery("select count(o) from Order o").UniqueResult();
            var itemCount = Session.CreateQuery("select count(i) from OrderItem i").UniqueResult();

            // oops... ordersWithItems contains an element for each OrderItem
            Assert.AreNotEqual(orderCount, ordersWithItems.Count);
            Assert.AreEqual(itemCount, ordersWithItems.Count);
        }

        [Test]
        public void fetch_join_on_one_to_many_with_distinct_result_transformer()
        {
            var ordersWithItems = Session.CreateQuery("from Order o join fetch o.Items")
                .SetResultTransformer(new DistinctRootEntityResultTransformer()).List<Order>();
            var orderCount = Session.CreateQuery("select count(o) from Order o").UniqueResult();

            // if you look at the output of the test, you'll notice that no distinct was used in the query
            Assert.AreEqual(orderCount, ordersWithItems.Count);
        }

        [Test]
        public void fetch_join_on_one_to_many_with_distinct_keyword()
        {
            var ordersWithItems = Session.CreateQuery("select distinct o from Order o join fetch o.Items").List<Order>();
            var orderCount = Session.CreateQuery("select count(o) from Order o").UniqueResult();

            // if you look at the output of the test, you'll notice that a distinct query was executed, which doesn't 
            // really make sense for the SQL statement we needed... the result is correct, but the query is slower
            // than it should be because of the distinct keyword
            Assert.AreEqual(orderCount, ordersWithItems.Count);
        }

        [Test]
        public void fetch_join_on_one_to_many_and_one_of_its_many_to_ones()
        {
            var ordersWithItemsAndProducts =
                Session.CreateQuery("from Order o join fetch o.Items items join fetch items.Product")
                    .SetResultTransformer(new DistinctRootEntityResultTransformer()).List<Order>();

            ordersWithItemsAndProducts.Each(o =>
            {
                // note: unlike the Criteria and QueryOver examples, HQL does not require us to use a left outer join
                // on OrderItems to make this test work
                Assert.IsTrue(NHibernateUtil.IsInitialized(ReflectionHelper.GetPrivateFieldValue(o, "_items")));
                o.Items.Each(i => Assert.IsTrue(NHibernateUtil.IsInitialized(i.Product)));
            });
        }
    }
}