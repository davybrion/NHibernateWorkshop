using Northwind.Builders;
using Northwind.Components;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Crud
{
    [TestFixture]
    public class SupplierCrud : CrudFixture<Supplier, int> 
    {
        protected override Supplier BuildEntity()
        {
            return new SupplierBuilder().Build();
        }

        protected override void ModifyEntity(Supplier entity)
        {
            var newAddress = new Address("some other street", "some other city", 32598, "some other country");

            entity.Address = newAddress;
            entity.ContactName = "some other contact";
            entity.Website = "http://somewebsite.com";
            entity.Fax = "123456789";
            entity.Name = "some other company name";
            entity.Phone = "987654321";
        }

        protected override void AssertAreEqual(Supplier expectedEntity, Supplier actualEntity)
        {
            Assert.AreEqual(expectedEntity.Address, actualEntity.Address);
            Assert.AreEqual(expectedEntity.ContactName, actualEntity.ContactName);
            Assert.AreEqual(expectedEntity.Website, actualEntity.Website);
            Assert.AreEqual(expectedEntity.Fax, actualEntity.Fax);
            Assert.AreEqual(expectedEntity.Name, actualEntity.Name);
            Assert.AreEqual(expectedEntity.Phone, actualEntity.Phone);
        }

        protected override void AssertValidId(Supplier entity)
        {
            Assert.That(entity.Id > 0);
        }
    }
}