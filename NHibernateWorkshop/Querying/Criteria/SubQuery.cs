using NHibernate.Criterion;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;
using Order = Northwind.Entities.Order;

namespace NHibernateWorkshop.Querying.Criteria
{
    [TestFixture]
    public class SubQuery : AutoRollbackFixture
    {
        private Employee _lazyEmployee;

        protected override void AfterSetUp()
        {
            _lazyEmployee = new EmployeeBuilder().Build();
            Session.Save(_lazyEmployee);
            FlushAndClear();
        }

        [Test]
        public void select_employees_with_no_orders()
        {
            var idsOfEmployeesWhoHaveOrders = DetachedCriteria.For<Order>()
                .SetProjection(Projections.Distinct(Projections.Property("Employee.Id")));

            var lazyEmployees = Session.CreateCriteria<Employee>()
                .Add(Subqueries.PropertyNotIn("Id", idsOfEmployeesWhoHaveOrders))
                .List<Employee>();

            Assert.That(lazyEmployees.Contains(_lazyEmployee));
        }
    }
}