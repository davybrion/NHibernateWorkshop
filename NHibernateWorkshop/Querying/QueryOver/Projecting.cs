using System;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
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
                    .Add(Projections.Property<Order>(o => o.OrderedOn).As("OrderedOn"))
                    .Add(Projections.Property(() => customer.Name).As("CustomerName"))
                    .Add(Projections.SqlFunction("concat", NHibernateUtil.String,
                            new[]
                                {
                                    Projections.Property(() => employee.FirstName),
                                    Projections.Constant(" "),
                                    Projections.Property(() => employee.LastName)
                                }), "EmployeeName"))
                .TransformUsing(new AliasToBeanResultTransformer(typeof(OrderHeader)))
                .List<OrderHeader>();

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