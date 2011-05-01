using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.AssociationFixtures.ManyToOneFixtures
{
    // Order.Customer is a unidirectional association with a save-update cascade setting
    // Order.Employee is a unidirectional association with no cascade setting
    [TestFixture]
    public class OrderCustomerEmployee : AutoRollbackFixture
    {
        private Order _order;
        private Customer _customer;
        private Employee _employee;

        protected override void AfterSetUp()
        {
            _order = new OrderBuilder().Build();
            _customer = _order.Customer;
            _employee = _order.Employee;
        }

        [Test] 
        public void saving_order_fails_when_employee_is_transient_reference()
        {
            var exception = Assert.Throws<NHibernate.PropertyValueException>(() => Session.Save(_order));
            Assert.IsTrue(exception.Message.Contains("Northwind.Entities.Order.Employee"));
        }

        private void SaveOrderAndEmployee()
        {
            Session.Save(_employee);
            Session.Save(_order);
            Flush();
        }

        [Test]
        public void saving_order_works_when_employee_is_made_persistent()
        {
            SaveOrderAndEmployee();
            Clear();

            // works because of our id-based equality implementation 
            Assert.AreEqual(_employee, Session.Get<Order>(_order.Id).Employee);
        }

        [Test]
        public void saving_order_cascades_to_transient_customer()
        {
            SaveOrderAndEmployee();
            Clear();

            // works because of our id-based equality implementation 
            Assert.AreEqual(_customer, Session.Get<Order>(_order.Id).Customer);
        }

        [Test]
        public void deleting_order_does_not_delete_customer()
        {
            SaveOrderAndEmployee();
            Session.Delete(_order);
            FlushAndClear();

            Assert.IsNull(Session.Get<Order>(_order.Id));
            Assert.IsNotNull(Session.Get<Customer>(_customer.Id));
        }

        [Test]
        public void deleting_order_does_not_delete_employee()
        {
            SaveOrderAndEmployee();
            Session.Delete(_order);
            FlushAndClear();

            Assert.IsNull(Session.Get<Order>(_order.Id));
            Assert.IsNotNull(Session.Get<Employee>(_employee.Id));
        }
    }
}