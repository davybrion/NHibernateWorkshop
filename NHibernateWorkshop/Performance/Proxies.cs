using NHibernate;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Performance
{
    [TestFixture]
    public class Proxies : AutoRollbackFixture
    {
        [Test]
        public void session_load_for_existing_entity()
        {
            var customer = new CustomerBuilder().Build();
            Session.Save(customer);
            FlushAndClear();

            var customerProxy = Session.Load<Customer>(customer.Id);
            Assert.IsFalse(NHibernateUtil.IsInitialized(customerProxy));
            Logger.Info("The select statement hasn't been executed yet, we just have a proxy to our customer");
            Logger.Info("Accessing its Id property doesn't trigger a select either");
            Logger.Info(string.Format("The Id is: {0}", customerProxy.Id));
            Logger.Info("Accessing any other property than the Id will issue the select statement however");
            Logger.Info(string.Format("The Name is: {0}", customerProxy.Name));
        }

        [Test]
        public void session_get_for_existing_entity()
        {
            var customer = new CustomerBuilder().Build();
            Session.Save(customer);
            FlushAndClear();

            Logger.Info("calling Session.Get will immediately issue the select statement");
            Session.Get<Customer>(customer.Id);
            Logger.Info("the select statement has been issued, even though we haven't done anything with the customer");
        }

        [Test]
        public void session_load_for_nonexisting_entity()
        {
            var nonExistingCustomerProxy = Session.Load<Customer>(99999);
            Logger.Info("Session.Load for a record that doesn't exist doesn't throw an exception");
            Logger.Info(string.Format("we can even access its Id property... Id: {0}", nonExistingCustomerProxy.Id));
            Logger.Info("accessing one of its other properties will issue the select statement, and then throw an exception");
            Assert.Throws<NHibernate.ObjectNotFoundException>(() => { var name = nonExistingCustomerProxy.Name; });
        }

        [Test]
        public void session_get_for_nonexisting_entity()
        {
            var nonExistingCustomer = Session.Get<Customer>(99999);
            Logger.Info("Session.Get for a nonexisting row doesn't throw an exception either, but nonExistingCustomer will be null");
            Assert.IsNull(nonExistingCustomer);
        }

        [Test]
        public void session_load_of_entity_that_was_already_present_in_session_cache()
        {
            var customer = new CustomerBuilder().Build();
            Session.Save(customer);
            FlushAndClear();

            var retrievedCustomer = Session.Get<Customer>(customer.Id);
            var customerProxy = Session.Load<Customer>(customer.Id);
            Assert.AreEqual(retrievedCustomer, customerProxy);
            Assert.AreSame(retrievedCustomer, customerProxy);
        }

        [Test]
        public void proxies_can_be_used_to_set_foreign_keys_without_loading_the_actual_entity()
        {
            var customer = new CustomerBuilder().Build();
            Session.Save(customer);
            FlushAndClear();

            var customerProxy = Session.Load<Customer>(customer.Id);
            Logger.Info("there was no select statement to retrieve the customer");
            var order = new OrderBuilder().WithCustomer(customerProxy).Build();
            Logger.Info("we can now insert the order without having to retrieve the customer");
            Session.Save(order.Employee);
            Session.Save(order);
            Flush();
        }

#if FLUENTSQLSERVER || HBMSQLSERVER // SQLite doesn't enforce foreign key constraints, so this test only works on databases that do enforce referential integrity
        [Test]
        public void proxies_cant_be_used_to_set_foreign_keys_with_values_that_dont_exist()
        {
            var nonExistingCustomerProxy = Session.Load<Customer>(9999);
            var order = new OrderBuilder().WithCustomer(nonExistingCustomerProxy).Build();
            Logger.Info("we can now insert the order without having to retrieve the customer");
            Session.Save(order.Employee);
            Session.Save(order);
            Assert.Throws<NHibernate.Exceptions.GenericADOException>(Flush);
        }
#endif

        [Test]
        public void many_to_one_properties_are_proxies_by_default()
        {
            var order = new OrderBuilder().Build();
            Session.Save(order.Employee);
            Session.Save(order);
            FlushAndClear();

            var retrievedOrder = Session.Get<Order>(order.Id);
            Assert.IsFalse(NHibernateUtil.IsInitialized(retrievedOrder.Customer));
            Assert.IsFalse(NHibernateUtil.IsInitialized(retrievedOrder.Employee));
            Logger.Info("if we access the Id values of both many-to-one properties, no select statement will be issued");
            Logger.Info(string.Format("Id of order.Customer: {0}", retrievedOrder.Customer.Id));
            Logger.Info(string.Format("Id of order.Employee: {0}", retrievedOrder.Employee.Id));
            Logger.Info("but once we access other properties, it does issue the select statement(s)");
            Logger.Info(string.Format("Name of order.Customer: {0}", retrievedOrder.Customer.Name));
            Logger.Info(string.Format("LastName of order.Employee: {0}", retrievedOrder.Employee.LastName));
        }
    }
}