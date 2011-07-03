using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Performance
{
    [TestFixture]
    public class StatelessSessions : Fixture
    {
        [Test]
        public void insert_hits_database_immediately()
        {
            using (var session = SessionFactory.OpenStatelessSession())
            using (var transaction = session.BeginTransaction())
            {
#if HBMSQLSERVER || FLUENTSQLSERVER
                session.SetBatchSize(0);
#endif
                var product = new ProductBuilder().Build();
                session.Insert(product);
                Logger.Info("the insert statement has been sent to the database before the session is flushed");
                transaction.Commit();
            }
        }

#if HBMSQLSERVER || FLUENTSQLSERVER
        [Test]
        public void inserts_are_batched()
        {
            using (var session = SessionFactory.OpenStatelessSession())
            using (var transaction = session.BeginTransaction())
            {
                session.SetBatchSize(2);
                var builder = new ProductBuilder();
                session.Insert(builder.Build());
                session.Insert(builder.Build());
                Logger.Info("the first batch of 2 inserts has been sent to the database now");
                session.Insert(builder.Build());
                session.Insert(builder.Build());
                Logger.Info("the second batch of 2 inserts has been sent to the database now, without flushing the session");
                transaction.Commit();
            }
        }
#endif

        [Test]
        public void update_hits_database_immediately()
        {
            using (var session = SessionFactory.OpenStatelessSession())
            using (var transaction = session.BeginTransaction())
            {
#if HBMSQLSERVER || FLUENTSQLSERVER
                session.SetBatchSize(0);
#endif
                var product = new ProductBuilder().Build();
                session.Insert(product);
                product.Name = product.Name + "2";
                session.Update(product);
                Logger.Info("the update statement has been sent to the database before the session is flushed");
                transaction.Commit();
            }
        }

#if HBMSQLSERVER || FLUENTSQLSERVER
        [Test]
        public void updates_are_batched()
        {
            using (var session = SessionFactory.OpenStatelessSession())
            using (var transaction = session.BeginTransaction())
            {
                session.SetBatchSize(2);
                var builder = new UserGroupBuilder();
                var group1 = builder.Build();
                var group2 = builder.Build();
                var group3 = builder.Build();
                var group4 = builder.Build();
                session.Insert(group1);
                session.Insert(group2);
                session.Insert(group3);
                session.Insert(group4);
                group1.Name = group1.Name + "2";
                group2.Name = group2.Name + "2";
                group3.Name = group3.Name + "2";
                group4.Name = group4.Name + "2";
                session.Update(group1);
                session.Update(group2);
                Logger.Info("the first batch of 2 update statements has been sent to the database");
                session.Update(group3);
                session.Update(group4);
                Logger.Info("the second batch of 2 update statements has been sent to the database, before the session is flushed");
                transaction.Commit();
            }
        }
#endif

        [Test]
        public void delete_hits_database_immediately()
        {
            using (var session = SessionFactory.OpenStatelessSession())
            using (var transaction = session.BeginTransaction())
            {
#if HBMSQLSERVER || FLUENTSQLSERVER
                session.SetBatchSize(0);
#endif
                var product = new ProductBuilder().Build();
                session.Insert(product);
                session.Delete(product);
                Logger.Info("the delete statement has been sent to the database before the session is flushed");
                transaction.Commit();
            }
        }

#if HBMSQLSERVER || FLUENTSQLSERVER
        [Test]
        public void deletes_are_batched()
        {
            using (var session = SessionFactory.OpenStatelessSession())
            using (var transaction = session.BeginTransaction())
            {
                session.SetBatchSize(2);
                var builder = new UserGroupBuilder();
                var group1 = builder.Build();
                var group2 = builder.Build();
                var group3 = builder.Build();
                var group4 = builder.Build();
                session.Insert(group1);
                session.Insert(group2);
                session.Insert(group3);
                session.Insert(group4);
                session.Delete(group1);
                session.Delete(group2);
                Logger.Info("the first batch of 2 delete statements has been sent to the database");
                session.Delete(group3);
                session.Delete(group4);
                Logger.Info("the second batch of 2 delete statements has been sent to the database, before the session is flushed");
                transaction.Commit();
            }
        }
#endif

        [Test]
        public void retrieving_the_same_entity_twice_hits_the_database_twice()
        {
            using (var session = SessionFactory.OpenStatelessSession())
            {
                var product = new ProductBuilder().Build();
                session.Insert(product);
                var firstRetrieval = session.Get<Product>(product.Id);
                Logger.Info("this issues a select statement, which shows that there is no session cache");
                var secondRetrieval = session.Get<Product>(product.Id);
                Logger.Info("this issues a second select statement...");
            }
        }

        [Test]
        public void no_dirty_checking()
        {
            var product = new ProductBuilder().Build();

            using (var session = SessionFactory.OpenStatelessSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Insert(product);
                product.Name = product.Name + "2";
                // with a normal NHibernate session, the changed name would've been detected by NHibernate
                // and an update statement would've been sent
                transaction.Commit();
            }

            using (var session = SessionFactory.OpenSession())
            {
                Assert.AreNotEqual(product.Name, session.Get<Product>(product.Id).Name);
            }
        }
    }
}