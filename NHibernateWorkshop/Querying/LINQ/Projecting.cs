using System;
using System.Linq;
using NHibernate.Linq;
using Northwind.Dtos;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.LINQ
{
    [TestFixture]
    public class Projecting : AutoRollbackFixture
    {
        [Test]
        public void project_into_dto()
        {
            var orderHeaders = Session.Query<Order>()
                .Select(o => new OrderHeader(o.OrderedOn, o.Customer.Name, o.Employee.FirstName + o.Employee.LastName))
                .ToList();

            Assert.Greater(orderHeaders.Count, 0);

            foreach (var orderHeader in orderHeaders)
            {
                Assert.IsNotNull(orderHeader.CustomerName);
                Assert.IsNotNull(orderHeader.EmployeeName);
                Assert.Greater(orderHeader.OrderedOn, DateTime.MinValue);
            }
        }
    }
}