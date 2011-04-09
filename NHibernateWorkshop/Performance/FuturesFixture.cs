using NHibernate.Criterion;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Performance
{
    [TestFixture]
    public class FuturesFixture : AutoRollbackFixture
    {
        [Test]
        public void future_list_queries_are_not_executed_until_a_result_is_accessed()
        {
            var suppliers = Session.QueryOver<Supplier>().Future();
            var customers = Session.QueryOver<Customer>().Future();
            Logger.Info("at this point, no query has actually been executed yet");
            Logger.Info("accessing the enumerator of either result triggers both queries in one roundtrip");
            suppliers.GetEnumerator().MoveNext();
        }

        [Test]
        public void future_scalar_queries_are_not_executed_until_a_result_is_accessed()
        {
            var supplierCount = Session.QueryOver<Supplier>().Select(Projections.RowCount()).FutureValue<int>();
            var customerCount = Session.QueryOver<Customer>().Select(Projections.RowCount()).FutureValue<int>();
            Logger.Info("at this point, no query has actually been executed yet");
            Logger.Info("accessing either of the results triggers both queries in one roundtrip");
            var blah = supplierCount.Value;
        }

    }
}