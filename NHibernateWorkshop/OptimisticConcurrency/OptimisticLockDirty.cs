using System;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.OptimisticConcurrency
{
    [TestFixture]
    public class OptimisticLockDirty : Fixture
    {
        private Employee _employee;

        [SetUp]
        public void SetUp()
        {
            _employee = new EmployeeBuilder().Build();
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
        public void simultaneous_changes_that_dont_conflict_are_possible()
        {
            using (var session1 = CreateSession())
            using (var session2 = CreateSession())
            {
                using (var trans1sess1 = session1.BeginTransaction())
                {
                    session1.Save(_employee);
                    trans1sess1.Commit();
                }

                using (var trans1sess2 = session2.BeginTransaction())
                {
                    var theEmployee = session2.Get<Employee>(_employee.Id);
                    theEmployee.HireDate = new DateTime(2007, 1, 1);
                    trans1sess2.Commit();
                }

                using (var trans2sess1 = session1.BeginTransaction())
                {
                    _employee.BirthDate = new DateTime(1979, 3, 1);
                    trans2sess1.Commit();
                }
            }
        }

        [Test]
        public void simultaneous_conflicting_changes_will_fail()
        {
            using (var session1 = CreateSession())
            using (var session2 = CreateSession())
            {
                using (var trans1sess1 = session1.BeginTransaction())
                {
                    session1.Save(_employee);
                    trans1sess1.Commit();
                }

                using (var trans1sess2 = session2.BeginTransaction())
                {
                    var theEmployee = session2.Get<Employee>(_employee.Id);
                    theEmployee.HireDate = new DateTime(2007, 1, 1);
                    trans1sess2.Commit();
                }

                using (var trans2sess1 = session1.BeginTransaction())
                {
                    _employee.HireDate = new DateTime(1979, 3, 1);
                    Assert.Throws<NHibernate.StaleObjectStateException>(() => trans2sess1.Commit());
                    trans2sess1.Rollback();
                }
            }
        }

        [Test]
        public void persisting_detached_entities_can_overwrite_concurrent_updates()
        {
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Save(_employee);
                transaction.Commit();
            }
            // as of this point, _employee is a detached instance

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                var theEmployee = session.Get<Employee>(_employee.Id);
                theEmployee.BirthDate = new DateTime(1979, 3, 1);
                transaction.Commit();
            }

            _employee.HireDate = new DateTime(2007, 1, 1);

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Update(_employee);
                transaction.Commit();
            }

            using (var session = CreateSession())
            {
                var theEmployee = session.Get<Employee>(_employee.Id);
                // the change to the BirthDate in another session has been overwritten without any exceptions
                Assert.AreEqual(_employee.BirthDate, theEmployee.BirthDate);
            }
        }

        [Test]
        public void persisting_merged_detached_entities_can_overwrite_concurrent_updates()
        {
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Save(_employee);
                transaction.Commit();
            }
            // as of this point, _employee is a detached instance

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                var theEmployee = session.Get<Employee>(_employee.Id);
                theEmployee.BirthDate = new DateTime(1979, 3, 1);
                transaction.Commit();
            }

            _employee.HireDate = new DateTime(2007, 1, 1);

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Merge(_employee);
                transaction.Commit();
            }

            using (var session = CreateSession())
            {
                var theEmployee = session.Get<Employee>(_employee.Id);
                // the change to the BirthDate in another session has been overwritten without any exceptions
                Assert.AreEqual(_employee.BirthDate, theEmployee.BirthDate);
            }
        }
    }
}