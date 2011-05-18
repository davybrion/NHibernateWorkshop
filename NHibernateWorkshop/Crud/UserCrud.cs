using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Crud
{
    [TestFixture]
    public class UserCrud : CrudFixture<User, int>
    {
        protected override User BuildEntity()
        {
            return new UserBuilder().Build();
        }

        protected override void ModifyEntity(User entity)
        {
            entity.UserName = "blah";
            entity.PasswordHash = new byte[] {3, 2, 1};
        }

        protected override void AssertAreEqual(User expectedEntity, User actualEntity)
        {
            Assert.AreEqual(expectedEntity.UserName, actualEntity.UserName);
            Assert.AreEqual(expectedEntity.PasswordHash, actualEntity.PasswordHash);
        }

        protected override void AssertValidId(User entity)
        {
            Assert.Greater(entity.Id, 0);
        }
    }
}