using System.Linq;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.AssociationFixtures.OneToManyFixtures
{
    // Order.Items is a unidirectional relationship where Order assumes full ownership of the relationship
    [TestFixture]
    public class OrderOrderItems : AutoRollbackFixture
    {
        private Order _order;
        private OrderItem _item1;
        private OrderItem _item2;

        protected override void AfterSetUp()
        {
            _order = new OrderBuilder().Build();
            _item1 = new OrderItemBuilder().Build();
            _item2 = new OrderItemBuilder().Build();

            // there are no cascades defined for the following relationships, so we have to save these manually
            Session.Save(_order.Employee);
            Session.Save(_item1.Product);
            Session.Save(_item2.Product);

            _order.AddItem(_item1);
            _order.AddItem(_item2);
            Session.Save(_order);
            Flush();
        }

        [Test]
        public void save_cascades_to_items()
        {
            Clear();
            var retrievedOrder = Session.Get<Order>(_order.Id);
            Assert.AreEqual(2, retrievedOrder.Items.Count());
            // the calls to Contain only work because our entities have id-based equality (check the Equals impl)
            Assert.True(retrievedOrder.Items.Contains(_item1));
            Assert.True(retrievedOrder.Items.Contains(_item2));
        }

        [Test]
        public void removing_item_from_order_removes_item_from_database()
        {
            _order.RemoveItem(_item1);
            FlushAndClear();

            Assert.IsNull(Session.Get<OrderItem>(_item1.Id));
        }

        [Test]
        public void removing_an_order_also_removes_its_items()
        {
            Session.Delete(_order);
            FlushAndClear();

            Assert.IsNull(Session.Get<Order>(_order.Id));
            Assert.IsNull(Session.Get<OrderItem>(_item1.Id));
            Assert.IsNull(Session.Get<OrderItem>(_item2.Id));
        }
    }
}