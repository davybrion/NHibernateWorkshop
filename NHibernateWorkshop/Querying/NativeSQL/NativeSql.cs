using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.NativeSQL
{
    [TestFixture]
    public class NativeSql : AutoRollbackFixture
    {
        [Test]
        public void select_entities_with_native_sql_query_through_session()
        {
            var products = Session.QueryOver<Product>().List();

            var productsThroughSql = Session.CreateSQLQuery("SELECT * FROM Product")
                .AddEntity(typeof(Product))
                .List<Product>();

            Assert.AreEqual(products, productsThroughSql);
        }

        [Test]
        public void select_scalar_value_with_native_sql_through_session()
        {
            var productCount = Session.QueryOver<Product>().RowCount();

            var productCountThroughSql = Session.CreateSQLQuery("SELECT COUNT(*) FROM Product")
                .UniqueResult();

            Assert.AreEqual(productCount, productCountThroughSql);
        }

#if HBMSQLITE || HBMSQLSERVER
        [Test]
        public void select_entities_with_native_sql_through_named_query()
        {
            var products = Session.QueryOver<Product>().List();
            var productsThroughNamedSqlQuery = Session.GetNamedQuery("GetAllProducts").List<Product>();
            Assert.AreEqual(products, productsThroughNamedSqlQuery);
        }

        [Test]
        public void select_scalar_value_with_native_sql_through_named_query()
        {
            var productCount = Session.QueryOver<Product>().RowCount();
            var productCountThroughSql = Session.GetNamedQuery("GetProductCount").UniqueResult();
            Assert.AreEqual(productCount, productCountThroughSql);
        }
#endif
    }
}