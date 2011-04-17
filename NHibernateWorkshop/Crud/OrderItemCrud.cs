using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Crud
{
    [TestFixture]
    public class OrderItemCrud : CrudFixture<OrderItem, int> 
    {
        protected override OrderItem BuildEntity()
        {
            var orderItem = new OrderItemBuilder().Build();
            // there are no cascade's configured for the following assocations, so we have to make these 
            // transient instances persistent manually
            Session.Save(orderItem.Product);
            return orderItem;
        }

        protected override void ModifyEntity(OrderItem entity)
        {
            entity.DiscountPercentage = 7.5d;
            entity.Quantity = 50;
            entity.UnitPrice = 0.60d;
        }

        protected override void AssertAreEqual(OrderItem expectedEntity, OrderItem actualEntity)
        {
            Assert.AreEqual(expectedEntity.DiscountPercentage, actualEntity.DiscountPercentage);
            Assert.AreEqual(expectedEntity.Product, actualEntity.Product);
            Assert.AreEqual(expectedEntity.Quantity, actualEntity.Quantity);
            Assert.AreEqual(expectedEntity.UnitPrice, actualEntity.UnitPrice);
        }

        protected override void AssertValidId(OrderItem entity)
        {
            Assert.That(entity.Id > 0);
        }
    }
}