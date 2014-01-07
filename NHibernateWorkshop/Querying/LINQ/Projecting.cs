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

        [Test]
        public void project_into_dto_with_collection()
        {
            var orderInfo = (from o in Session.Query<Order>()
                             select new
                             {
                                 o.OrderedOn,
                                 CustomerName = o.Customer.Name,
                                 EmployeeName = o.Employee.FirstName + o.Employee.LastName,
                                 OrderItems = (from oi in o.Items
                                               select new
                                               {
                                                   ProductName = oi.Product.Name,
                                                   oi.Quantity,
                                                   oi.UnitPrice,
                                                   oi.DiscountPercentage
                                               })
                             }).ToList();

            Assert.Greater(orderInfo.Count, 0);

            foreach (var info in orderInfo)
            {
                Assert.IsNotNull(info.CustomerName);
                Assert.IsNotNull(info.EmployeeName);
                Assert.Greater(info.OrderedOn, DateTime.MinValue);
                Assert.Greater(info.OrderItems.Count(), 0);
            }
        }
    }
}