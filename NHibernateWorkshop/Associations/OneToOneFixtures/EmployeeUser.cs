using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Associations.OneToOneFixtures
{
    [TestFixture]
    public class EmployeeUser : AutoRollbackFixture
    {
        private Employee _employee;
        private User _user;

        protected override void AfterSetUp()
        {
            _employee = new EmployeeBuilder().Build();
            _user = new UserBuilder().WithEmployee(_employee).Build();
            _employee.User = _user; // have to keep both sides of the relationship in synch
        }

        [Test]
        public void saving_employee_also_saves_user()
        {
            Session.Save(_employee);
            FlushAndClear();

            Assert.AreEqual(_user, Session.Get<Employee>(_employee.Id).User);
        }

        [Test]
        public void user_can_be_saved_directly()
        {
            Session.Save(_user);
            FlushAndClear();

            Assert.AreEqual(_employee, Session.Get<User>(_user.Id).Employee);
        }

        [Test]
        public void deleting_employee_also_deletes_the_user()
        {
            Session.Save(_employee);
            Flush();

            Session.Delete(_employee);
            FlushAndClear();

            Assert.IsNull(Session.Get<Employee>(_employee.Id));
            Assert.IsNull(Session.Get<User>(_user.Id));
        }

        [Test]
        public void deleting_user_does_not_delete_employee()
        {
            Session.Save(_employee);
            Flush();

            Session.Delete(_user);
            _employee.User = null; // you have to set this association to null, or it would be re-saved because of the cascades
            FlushAndClear();

            Assert.IsNull(Session.Get<User>(_user.Id));
            Assert.IsNotNull(Session.Get<Employee>(_employee.Id));
        }
    }
}