using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.QueryOver
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
            var idsOfEmployeesWhoHaveOrders = NHibernate.Criterion.QueryOver.Of<Order>()
                .Select(o => o.Employee.Id);

            var lazyEmployees = Session.QueryOver<Employee>()
                .WithSubquery.WhereProperty(e => e.Id).NotIn(idsOfEmployeesWhoHaveOrders)
                .List();

            Assert.That(lazyEmployees.Contains(_lazyEmployee));
        }
    }
}