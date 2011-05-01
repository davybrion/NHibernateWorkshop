using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Crud
{
    public abstract class CrudFixture<TEntity, TId> : AutoRollbackFixture 
        where TEntity : Entity<TId> 
    {
        [Test]
        public void can_select_entity()
        {
            Session.CreateCriteria<TEntity>().SetMaxResults(1).List();
        }

        [Test]
        public void can_create_entity()
        {
            var entity = BuildEntity();
            InsertEntity(entity);
            Session.Evict(entity);

            var reloadedEntity = Session.Get<TEntity>(entity.Id);
            Assert.IsNotNull(reloadedEntity);
            AssertAreEqual(entity, reloadedEntity);
            AssertValidId(entity);
        }

        [Test]
        public void can_update_entity()
        {
            var entity = BuildEntity();
            InsertEntity(entity);
            ModifyEntity(entity);
            Session.Flush();
            Session.Evict(entity);

            var reloadedEntity = Session.Get<TEntity>(entity.Id);
            AssertAreEqual(entity, reloadedEntity);
        }

        [Test]
        public void can_delete_entity()
        {
            var entity = BuildEntity();
            InsertEntity(entity);
            DeleteEntity(entity);
            Assert.IsNull(Session.Get<TEntity>(entity.Id));
        }

        protected virtual void InsertEntity(TEntity entity)
        {
            Session.Save(entity);
            Session.Flush();
        }

        protected virtual void DeleteEntity(TEntity entity)
        {
            Session.Delete(entity);
            Session.Flush();
        }

        protected abstract TEntity BuildEntity();
        protected abstract void ModifyEntity(TEntity entity);
        protected abstract void AssertAreEqual(TEntity expectedEntity, TEntity actualEntity);
        protected abstract void AssertValidId(TEntity entity);
    }
}