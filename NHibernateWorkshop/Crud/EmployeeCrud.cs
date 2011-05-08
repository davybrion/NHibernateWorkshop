using Northwind.Builders;
using Northwind.Components;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Crud
{
    [TestFixture]
    public class EmployeeCrud : CrudFixture<Employee, int> 
    {
        protected override Employee BuildEntity()
        {
            return new EmployeeBuilder().Build();
        }

        protected override void ModifyEntity(Employee entity)
        {
            var newAddress = new Address("some other street", "some other city", 32598, "some other country");

            var manager = new EmployeeBuilder()
                .WithFirstName("Tim")
                .WithLastName("Jackson")
                .Build();

            Session.Save(manager);

            entity.Address = newAddress;
            entity.BirthDate = entity.BirthDate.AddDays(1);
            entity.FirstName = "Jimmy";
            entity.HireDate = entity.HireDate.AddDays(2);
            entity.LastName = "O'Donel";
            entity.Manager = manager;
            entity.Salary = 2500;
        }

        protected override void AssertAreEqual(Employee expectedEntity, Employee actualEntity)
        {
            Assert.AreEqual(expectedEntity.Address, actualEntity.Address);
            Assert.AreEqual(expectedEntity.BirthDate, actualEntity.BirthDate);
            Assert.AreEqual(expectedEntity.FirstName, actualEntity.FirstName);
            Assert.AreEqual(expectedEntity.HireDate, actualEntity.HireDate);
            Assert.AreEqual(expectedEntity.LastName, actualEntity.LastName);
            Assert.AreEqual(expectedEntity.Manager, actualEntity.Manager);
            Assert.AreEqual(expectedEntity.Phone, actualEntity.Phone);
            Assert.AreEqual(expectedEntity.Title, actualEntity.Title);
            Assert.AreEqual(expectedEntity.Salary, actualEntity.Salary);
        }

        protected override void AssertValidId(Employee entity)
        {
            Assert.That(entity.Id > 0);
        }
    }
}