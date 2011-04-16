using NHibernate.Criterion;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Performance
{
    [TestFixture]
    public class Futures : AutoRollbackFixture
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

        [Test]
        public void future_get_by_id_retrieves_selected_record_only_when_it_is_accessed()
        {
            var newCustomer = new CustomerBuilder().Build();
            Session.Save(newCustomer);
            FlushAndClear();
            var retrievedNewCustomer = Session.QueryOver<Customer>().Where(c => c.Id == newCustomer.Id).FutureValue();
            Logger.Info("at this point, the select statement has not been executed yet");
            Logger.Info("it will be executed once we try to access the retrieved value");
            var id = retrievedNewCustomer.Value.Id;
        }
    }
}