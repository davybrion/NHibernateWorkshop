using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.OptimisticConcurrency
{
    [TestFixture]
    public class Version : Fixture
    {
        private Product _product;

        [SetUp]
        public void SetUp()
        {
            _product = new ProductBuilder().Build();
        }

        [TearDown]
        public void TearDown()
        {
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                session
                    .CreateQuery("delete from Product p where p.Id = :id")
                    .SetParameter("id", _product.Id)
                    .ExecuteUpdate();

                transaction.Commit();
            }
        }

        [Test]
        public void version_increments_with_each_persist()
        {
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                Assert.AreEqual(0, _product.Version);
                session.Save(_product);
                session.Flush();
                Assert.AreEqual(1, _product.Version);

                _product.UnitPrice = 10;
                session.Flush();
                Assert.AreEqual(2, _product.Version);
                transaction.Commit();
            }
        }

        [Test]
        public void update_of_instance_with_old_version_number_fails()
        {
            using (var session1 = CreateSession())
            using (var session2 = CreateSession())
            {
                using (var trans1sess1 = session1.BeginTransaction())
                {
                    session1.Save(_product);
                    trans1sess1.Commit();
                }

                using (var trans1sess2 = session2.BeginTransaction())
                {
                    var theProduct = session2.Get<Product>(_product.Id);
                    theProduct.UnitsInStock = 50;
                    trans1sess2.Commit();
                }

                using (var trans2sess1 = session1.BeginTransaction())
                {
                    _product.UnitsOnOrder = 20;
                    // committing the transaction will throw a StaleObjectStateException because another transaction
                    // has updated the same product record, and its version number in the database is larger than the
                    // value we have in _product.Version, which means it's an outdated instance.
                    Assert.Throws<NHibernate.StaleObjectStateException>(() => trans2sess1.Commit());
                    trans2sess1.Rollback();
                }
            }
        }

        [Test]
        public void refresh_of_instance_with_old_version_number_enables_new_updates()
        {
            using (var session1 = CreateSession())
            using (var session2 = CreateSession())
            {
                using (var trans1sess1 = session1.BeginTransaction())
                {
                    session1.Save(_product);
                    trans1sess1.Commit();
                    Assert.AreEqual(1, _product.Version);
                }

                using (var trans1sess2 = session2.BeginTransaction())
                {
                    var theProduct = session2.Get<Product>(_product.Id);
                    theProduct.UnitsInStock = 50;
                    trans1sess2.Commit();
                    Assert.AreEqual(2, theProduct.Version);
                }

                using (var trans2sess1 = session1.BeginTransaction())
                {
                    Assert.AreEqual(1, _product.Version);
                    session1.Refresh(_product);
                    // if we had changed UnitsOnOrder before the call to Refresh, its value would've been overwritten
                    // with the value in the latest version in the database (NULL)
                    _product.UnitsOnOrder = 20;
                    Assert.AreEqual(2, _product.Version);
                    Assert.AreEqual(50, _product.UnitsInStock);
                    trans2sess1.Commit();
                    Assert.AreEqual(20, _product.UnitsOnOrder);
                }
            }
        }

        [Test]
        public void version_check_works_with_detached_entities()
        {
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Save(_product);
                transaction.Commit();
            }
            // as of this point, _product is a detached entity

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                var theProduct = session.Get<Product>(_product.Id);
                theProduct.UnitsOnOrder = 20;
                transaction.Commit();
            }

            _product.UnitsInStock = 50;

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Update(_product);
                Assert.Throws<NHibernate.StaleObjectStateException>(() => transaction.Commit());
                transaction.Rollback();
            }
        }
    }
}