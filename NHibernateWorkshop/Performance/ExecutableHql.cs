using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Performance
{
    [TestFixture]
    public class ExecutableHql : AutoRollbackFixture
    {
        [Test]
        public void can_delete_multiple_records_with_one_statement()
        {
            var count = Session.QueryOver<OrderItem>()
                .Where(p => p.Quantity == 0)
                .RowCount();

            var deletedProducts =
                Session.CreateQuery("delete OrderItem i where i.Quantity = 0")
                    .ExecuteUpdate();

            Assert.AreEqual(count, deletedProducts);

            count = Session.QueryOver<OrderItem>()
                            .Where(p => p.Quantity == 0)
                            .RowCount();

            Assert.AreEqual(0, count);
        }

        [Test]
        public void can_update_multiple_records_with_one_statement()
        {
            var count = Session.QueryOver<Product>()
                .Where(p => p.UnitsOnOrder == 0 || p.UnitsOnOrder == null)
                .RowCount();

            var updatedProducts =
                Session.CreateQuery("update Product p set p.Discontinued = true where p.UnitsOnOrder = 0 or p.UnitsOnOrder is null")
                    .ExecuteUpdate();

            Assert.AreEqual(count, updatedProducts);
        }
    }
}