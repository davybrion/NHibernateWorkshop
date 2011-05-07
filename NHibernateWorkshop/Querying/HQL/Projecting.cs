using System;
using NHibernate.Transform;
using Northwind.Dtos;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.HQL
{
    [TestFixture]
    public class Projecting : AutoRollbackFixture
    {
        [Test]
        public void project_into_dto()
        {
#if HBMSQLITE || HBMSQLSERVER || (DEBUG && !FLUENTSQLITE && !FLUENTSQLSERVER)
            var orderHeaders = Session.CreateQuery(
                @"select new OrderHeader(o.OrderedOn, c.Name, e.FirstName || ' ' || e.LastName)
                    from Order o inner join o.Customer c inner join o.Employee e")
                .List<OrderHeader>();
#else
            var orderHeaders = Session.CreateQuery(
                @"select o.OrderedOn as OrderedOn, c.Name as CustomerName, e.FirstName || ' ' || e.LastName as EmployeeName
                    from Order o inner join o.Customer c inner join o.Employee e")
                .SetResultTransformer(new AliasToBeanResultTransformer(typeof(OrderHeader)))
                .List<OrderHeader>();
#endif
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