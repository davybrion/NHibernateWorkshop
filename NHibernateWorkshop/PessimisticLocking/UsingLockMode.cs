using NHibernate;
using NHibernate.Exceptions;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

#if HBMSQLSERVER // SQLite doesn't support locks and haven't found a clean way to set command timeout with FluentNHibernate 

namespace NHibernateWorkshop.PessimisticLocking
{
    [TestFixture]
    [Explicit("These tests are slow since they trigger timeouts on the database (on purpose, obviously :p)")]
    public class UsingLockMode : Fixture
    {
        private Employee _employee;

        [SetUp]
        public void SetUp()
        {
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                _employee = new EmployeeBuilder().Build();
                session.Save(_employee);
                transaction.Commit();
            }
        }

        [TearDown]
        public void TearDown()
        {
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                session
                    .CreateQuery("delete from Employee e where e.Id = :id")
                    .SetParameter("id", _employee.Id)
                    .ExecuteUpdate();

                transaction.Commit();
            }
        }

        [Test]
        public void locking_a_specific_entity_with_upgrade_lock_makes_it_unavailable_to_other_session_with_upgrade_lock()
        {
            using (var session1 = CreateSession())
            using (var session2 = CreateSession())
            using (var transaction1 = session1.BeginTransaction())
            using (var transaction2 = session2.BeginTransaction())
            {
                var employeeViaSession1 = session1.Get<Employee>(_employee.Id, LockMode.Upgrade);
                // this will timeout after the command timeout has expired (is set to 2 seconds in sqlserver.hibernate.cfg.xml)
                var exception = Assert.Catch<GenericADOException>(() => session2.Get<Employee>(_employee.Id, LockMode.Upgrade));
                Assert.IsTrue(exception.InnerException.Message.Contains("Timeout expired"));
            }
        }

        [Test]
        public void locking_a_specific_entity_with_upgrade_lock_prevents_modification_in_other_session()
        {
            using (var session1 = CreateSession())
            using (var session2 = CreateSession())
            using (var transaction1 = session1.BeginTransaction())
            using (var transaction2 = session2.BeginTransaction())
            {
                var employeeViaSession1 = session1.Get<Employee>(_employee.Id, LockMode.Upgrade);
                var employeeviaSession2 = session2.Get<Employee>(_employee.Id);
                employeeviaSession2.Title = "something fancy";
                var exception = Assert.Catch<GenericADOException>(() => transaction2.Commit());
                Assert.IsTrue(exception.InnerException.Message.Contains("Timeout expired"));
            }
        }

        [Test]
        public void locking_a_set_of_records_with_upgrade_lock_prevents_them_from_being_modified_in_other_session()
        {
            using (var session1 = CreateSession())
            using (var session2 = CreateSession())
            using (var transaction1 = session1.BeginTransaction())
            using (var transaction2 = session2.BeginTransaction())
            {
                // all of the selected employees will be locked
                var employeesWithLock = session1.QueryOver<Employee>()
                    .Lock().Upgrade
                    .List();

                var employeeviaSession2 = session2.Get<Employee>(_employee.Id);
                employeeviaSession2.Title = "something fancy";
                var exception = Assert.Catch<GenericADOException>(() => transaction2.Commit());
                Assert.IsTrue(exception.InnerException.Message.Contains("Timeout expired"));
            }
        }
    }
}

#endif