using System;
using Northwind.Builders;
using Northwind.Entities;
using Northwind.Enums;
using NUnit.Framework;

namespace NHibernateWorkshop.Crud
{
    [TestFixture]
    public class ProductCrud : CrudFixture<Product, Guid> 
    {
        protected override Product BuildEntity()
        {
            return new ProductBuilder().Build();
        }

        protected override void ModifyEntity(Product entity)
        {
            entity.Category = ProductCategory.Condiments;
            entity.Discontinued = true;
            entity.Name = "Sugar";
            entity.ReorderLevel = 10;
            entity.UnitPrice = 1.20d;
            entity.UnitsInStock = 10;
            entity.UnitsOnOrder = 7;
        }

        protected override void AssertAreEqual(Product expectedEntity, Product actualEntity)
        {
            Assert.AreEqual(expectedEntity.Category, actualEntity.Category);
            Assert.AreEqual(expectedEntity.Discontinued, actualEntity.Discontinued);
            Assert.AreEqual(expectedEntity.Name, actualEntity.Name);
            Assert.AreEqual(expectedEntity.ReorderLevel, actualEntity.ReorderLevel);
            Assert.AreEqual(expectedEntity.UnitPrice, actualEntity.UnitPrice);
            Assert.AreEqual(expectedEntity.UnitsInStock, actualEntity.UnitsInStock);
            Assert.AreEqual(expectedEntity.UnitsOnOrder, actualEntity.UnitsOnOrder);
        }

        protected override void AssertValidId(Product entity)
        {
            // irrelevant for assigned guid id
        }
    }
}