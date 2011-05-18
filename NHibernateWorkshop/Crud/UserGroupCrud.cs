using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Crud
{
    [TestFixture]
    public class UserGroupCrud : CrudFixture<UserGroup, int>
    {
        protected override UserGroup BuildEntity()
        {
            return new UserGroupBuilder().Build();
        }

        protected override void ModifyEntity(UserGroup entity)
        {
            entity.Name = "guests";
        }

        protected override void AssertAreEqual(UserGroup expectedEntity, UserGroup actualEntity)
        {
            Assert.AreEqual(expectedEntity.Name, actualEntity.Name);
        }

        protected override void AssertValidId(UserGroup entity)
        {
            Assert.Greater(entity.Id, 0);
        }
    }
}