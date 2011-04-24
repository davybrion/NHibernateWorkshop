using System;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Northwind.Dtos;
using NUnit.Framework;
using Order = Northwind.Entities.Order;

namespace NHibernateWorkshop.Querying.Criteria
{
    [TestFixture]
    public class Projecting : AutoRollbackFixture
    {
        [Test]
        public void project_into_dto()
        {
            var orderHeaders = Session.CreateCriteria<Order>("order")
                .CreateCriteria("order.Employee", "employee")
                .CreateCriteria("order.Customer", "customer")
                .SetProjection(Projections.ProjectionList()
                                   .Add(Projections.Property("order.OrderedOn"), "OrderedOn")
                                   .Add(Projections.SqlFunction("concat", NHibernateUtil.String,
                                                                new[]
                                                                    {
                                                                        Projections.Property("employee.FirstName"),
                                                                        Projections.Constant(" "),
                                                                        Projections.Property("employee.LastName")
                                                                    }), "EmployeeName")
                                   .Add(Projections.Property("customer.Name"), "CustomerName")
                )
                .SetResultTransformer(new AliasToBeanResultTransformer(typeof(OrderHeader)))
                .List<OrderHeader>();

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