using System.Linq;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernateWorkshop.Querying.LINQ
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
            var idsOfEmployeesWhoHaveOrders = Session.Query<Order>()
                .Select(o => o.Employee.Id);

            var lazyEmployees = Session.Query<Employee>()
                .Where(e => idsOfEmployeesWhoHaveOrders.Count(i => i == e.Id) == 0)
                .ToArray();

            Assert.That(lazyEmployees.Contains(_lazyEmployee));
        }
    }
}