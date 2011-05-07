using System.Linq;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.AssociationFixtures.OneToManyFixtures
{
    // Employees.Subordinates is the inverse end of a bidirectional relationship, with save-update cascading
    [TestFixture]
    public class EmployeeSubordinates : AutoRollbackFixture
    {
        private Employee _manager;
        private Employee _subordinate1;
        private Employee _subordinate2;

        protected override void AfterSetUp()
        {
            _manager = new EmployeeBuilder().Build();

            _subordinate1 = new EmployeeBuilder()
                .WithFirstName("Some")
                .WithLastName("Dude")
                .Build();

            _subordinate2 = new EmployeeBuilder()
                .WithFirstName("Some Other")
                .WithLastName("Dude")
                .Build();

            _manager.AddSubordinate(_subordinate1);
            _manager.AddSubordinate(_subordinate2);
            Session.Save(_manager);
            Flush();
        }

        [Test]
        public void save_cascades_to_subordinates()
        {
            Clear();

            var retrievedManager = Session.Get<Employee>(_manager.Id);
            Assert.AreEqual(2, retrievedManager.Subordinates.Count());
            // the calls to Contain only work because our entities have id-based equality (check the Equals impl)
            Assert.True(retrievedManager.Subordinates.Contains(_subordinate1));
            Assert.True(retrievedManager.Subordinates.Contains(_subordinate2));
        }

        [Test]
        public void removing_subordinate_from_manager_does_not_delete_it_from_database()
        {
            _manager.RemoveSubordinate(_subordinate1);
            FlushAndClear();

            var retrievedManager = Session.Get<Employee>(_manager.Id);
            var retrievedSubordinate1 = Session.Get<Employee>(_subordinate1.Id);
            // the calls to Contain only work because our entities have id-based equality (check the Equals impl)
            Assert.False(retrievedManager.Subordinates.Contains(retrievedSubordinate1));
            Assert.True(retrievedManager.Subordinates.Contains(_subordinate2));
            Assert.IsNull(retrievedSubordinate1.Manager);
        }

        [Test]
        public void removing_manager_does_not_remove_subordinates_from_database()
        {
            Session.Delete(_manager);
            // setting the Manager reference to null is necessary because Employee.Subordinates is 
            // the inverse end of the bidirectional relationship
            _subordinate1.Manager = null;
            _subordinate2.Manager = null;
            FlushAndClear();

            var retrievedSubordinate1 = Session.Get<Employee>(_subordinate1.Id);
            var retrievedSubordinate2 = Session.Get<Employee>(_subordinate2.Id);
            Assert.IsNotNull(retrievedSubordinate1);
            Assert.IsNotNull(retrievedSubordinate2);
            Assert.IsNull(retrievedSubordinate1.Manager);
            Assert.IsNull(retrievedSubordinate2.Manager);
        }
    }
}