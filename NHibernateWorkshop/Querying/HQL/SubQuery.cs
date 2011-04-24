using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.HQL
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
            var lazyEmployees =
                Session.CreateQuery(
                    "from Employee e where e.Id not in (select order.Employee.Id from Order order)")
                    .List<Employee>();

            Assert.That(lazyEmployees.Contains(_lazyEmployee));
        }
    }
}