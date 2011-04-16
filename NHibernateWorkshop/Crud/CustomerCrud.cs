using Northwind.Builders;
using Northwind.Components;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Crud
{
    [TestFixture]
    public class CustomerCrud : CrudFixture<Customer, int> 
    {
        protected override Customer BuildEntity()
        {
            return new CustomerBuilder().Build();
        }

        protected override void ModifyEntity(Customer entity)
        {
            var newAddress = new Address("some other street", "some other city", 32598, "some other country");

            entity.Address = newAddress;
            entity.ContactName = "some other contact";
            entity.DiscountPercentage = 4.5d;
            entity.Fax = "123456789";
            entity.Name = "some other company name";
            entity.Phone = "987654321";
        }

        protected override void AssertAreEqual(Customer expectedEntity, Customer actualEntity)
        {
            Assert.AreEqual(expectedEntity.Address, actualEntity.Address);
            Assert.AreEqual(expectedEntity.ContactName, actualEntity.ContactName);
            Assert.AreEqual(expectedEntity.DiscountPercentage, actualEntity.DiscountPercentage);
            Assert.AreEqual(expectedEntity.Fax, actualEntity.Fax);
            Assert.AreEqual(expectedEntity.Name, actualEntity.Name);
            Assert.AreEqual(expectedEntity.Phone, actualEntity.Phone);
        }

        protected override void AssertValidId(Customer entity)
        {
            Assert.That(entity.Id > 0);
        }
    }
}