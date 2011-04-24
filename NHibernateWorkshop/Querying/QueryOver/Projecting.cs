using System;
using System.Linq;
using NHibernate.Criterion;
using Northwind.Dtos;
using Northwind.Entities;
using NUnit.Framework;
using Order = Northwind.Entities.Order;

namespace NHibernateWorkshop.Querying.QueryOver
{
    [TestFixture]
    public class Projecting : AutoRollbackFixture
    {
        [Test]
        public void project_into_dto()
        {
            Customer customer = null;
            Employee employee = null;

            var orderHeaders = Session.QueryOver<Order>()
                .JoinAlias(o => o.Customer, () => customer)
                .JoinAlias(o => o.Employee, () => employee)
                .Select(Projections.ProjectionList()
                    .Add(Projections.Property<Order>(o => o.OrderedOn))
                    .Add(Projections.Property(() => customer.Name))
                    .Add(Projections.Property(() => employee.FirstName))
                    .Add(Projections.Property(() => employee.LastName)))
                .List<Object[]>()
                // the following select is the .NET Select extension method on IEnumerable, not part of the QueryOver api
                .Select(values => new OrderHeader((DateTime)values[0], (string)values[1], (string)values[2] + " " + (string)values[3]));

            Assert.Greater(orderHeaders.Count(), 0);

            foreach (var orderHeader in orderHeaders)
            {
                Assert.IsNotNull(orderHeader.CustomerName);
                Assert.IsNotNull(orderHeader.EmployeeName);
                Assert.Greater(orderHeader.OrderedOn, DateTime.MinValue);
            }
        }
    }
}