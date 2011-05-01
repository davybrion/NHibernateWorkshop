using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.AssociationFixtures.ManyToOneFixtures
{
    // there is no cascade defined for Employee.Manager
    // keep in mind that any manipulation of the Employee.Manager association is NOT reflected
    // in the manager's Subordinates collection (the other side of this bidirectional association)!
    [TestFixture]
    public class EmployeeManager : AutoRollbackFixture
    {
        private Employee _employee;
        private Employee _manager;

        protected override void AfterSetUp()
        {
            _employee = new EmployeeBuilder().Build();

            _manager = new EmployeeBuilder()
                .WithFirstName("Steve")
                .WithLastName("Ballmer")
                .Build();
        }

        [Test]
        public void saving_employee_fails_when_manager_is_transient_reference()
        {
            _employee.Manager = _manager;
            Session.Save(_employee);

            Assert.Throws<NHibernate.TransientObjectException>(() => Flush());
        }

        private void SaveEmployeeAndManager()
        {
            _employee.Manager = _manager;
            Session.Save(_manager);
            Session.Save(_employee);
            Flush();
        }

        [Test]
        public void saving_employee_works_when_manager_is_made_persistent()
        {
            SaveEmployeeAndManager();
            Clear();

            var retrievedEmployee = Session.Get<Employee>(_employee.Id);
            // works because of our id-based equality implementation 
            Assert.AreEqual(_manager, retrievedEmployee.Manager);
        }

        [Test]
        public void deleting_manager_requires_update_of_employee()
        {
            SaveEmployeeAndManager();
            Session.Delete(_manager);
            _employee.Manager = null;
            FlushAndClear();

            Assert.IsNull(Session.Get<Employee>(_manager.Id));
            Assert.IsNull(Session.Get<Employee>(_employee.Id).Manager);
        }

        [Test]
        public void deleting_employee_does_not_delete_manager()
        {
            SaveEmployeeAndManager();
            Session.Delete(_employee);
            FlushAndClear();

            Assert.IsNull(Session.Get<Employee>(_employee.Id));
            Assert.IsNotNull(Session.Get<Employee>(_manager.Id));
        }
    }
}