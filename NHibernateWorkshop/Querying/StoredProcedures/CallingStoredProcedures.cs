using System.Linq;
using NHibernate.Criterion;
using Northwind.Entities;
using NUnit.Framework;

#if HBMSQLSERVER

namespace NHibernateWorkshop.Querying.StoredProcedures
{
    [TestFixture]
    [Explicit("Read the README.txt file in this folder")]
    public class CallingStoredProcedures : AutoRollbackFixture
    {
        [Test]
        public void call_stored_procedure_that_returns_entities()
        {
            var allEmployees = Session.QueryOver<Employee>()
                .OrderBy(e => e.Salary).Asc.List();

            var employees = Session.GetNamedQuery("GET_EMPLOYEES_WITH_HIGHER_SALARY_THAN")
                .SetParameter("employeeId", allEmployees[0].Id)
                .List<Employee>();

            Assert.IsFalse(employees.Contains(allEmployees[0]));
            allEmployees.Skip(1).Each(e => Assert.IsTrue(employees.Contains(e)));
        }

        [Test]
        public void call_stored_procedure_that_returns_non_entity_value()
        {
            var averageSalary = Session.QueryOver<Employee>()
                .Select(Projections.Avg<Employee>(e => e.Salary))
                .SingleOrDefault<double>();

            Assert.AreEqual(averageSalary, Session.GetNamedQuery("GET_AVERAGE_SALARY").UniqueResult());
        }
    }
}

#endif