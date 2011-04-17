using System;
using Northwind.Builders;
using Northwind.Components;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Crud
{
    [TestFixture]
    public class OrderCrud : CrudFixture<Order, Guid> 
    {
        protected override Order BuildEntity()
        {
            var order = new OrderBuilder().Build();
            // there's no cascade set on Order's Employee association so we first have to make the transient order.Employee instance persistent
            // before the order can be made persistent
            Session.Save(order.Employee);
            return order;
        }

        protected override void ModifyEntity(Order entity)
        {
            var otherEmployee = new EmployeeBuilder()
                .WithFirstName("Tim")
                .WithLastName("Jackson")
                .Build();

            var otherCustomer = new CustomerBuilder()
                .WithName("Acme's biggest competitor")
                .Build();

            Session.Save(otherEmployee);
            Session.Save(otherCustomer);

            entity.Customer = otherCustomer;
            entity.DeliveryAddress = new Address("some other street", "some other city", 1235, "some other country");
            entity.Employee = otherEmployee;
            entity.OrderedOn = DateTime.Now.AddMinutes(30);
            entity.ShippedOn = DateTime.Now.AddDays(2);
        }

        protected override void AssertAreEqual(Order expectedEntity, Order actualEntity)
        {
            Assert.AreEqual(expectedEntity.Customer, actualEntity.Customer);
            Assert.AreEqual(expectedEntity.DeliveryAddress, actualEntity.DeliveryAddress);
            Assert.AreEqual(expectedEntity.Employee, actualEntity.Employee);
            Assert.AreEqual(expectedEntity.OrderedOn.RemoveMillies(), actualEntity.OrderedOn.RemoveMillies());
            Assert.AreEqual(expectedEntity.ShippedOn.RemoveMillies(), actualEntity.ShippedOn.RemoveMillies());
        }

        protected override void AssertValidId(Order entity)
        {
            Assert.AreNotEqual(Guid.Empty, entity.Id);
        }
    }
}