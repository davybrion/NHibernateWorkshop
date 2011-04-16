using NHibernate.Criterion;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.Criteria
{
    [TestFixture]
    public class UniqueResult : AutoRollbackFixture
    {
        private const string _customerName = "TestCustomer";
        private readonly DetachedCriteria _query = DetachedCriteria.For<Customer>()
                .Add(Restrictions.Eq("Name", _customerName));

        [Test]
        public void unique_result_returns_one_value_instead_of_list_with_one_value()
        {
            Session.Save(new CustomerBuilder().WithName(_customerName).Build());
            FlushAndClear();

            Logger.Info("The following call executes the query and returns a list of values, even though there's only one");
            Assert.AreEqual(_customerName, _query.GetExecutableCriteria(Session).List<Customer>()[0].Name);
            Logger.Info("Calling UniqueResult instead of list returns just one value, even though the select statement will be the same");
            Assert.AreEqual(_customerName, _query.GetExecutableCriteria(Session).UniqueResult<Customer>().Name);
        }

        [Test]
        public void unique_result_fails_when_resultset_contains_more_than_one_row()
        {
            Session.Save(new CustomerBuilder().WithName(_customerName).Build());
            Session.Save(new CustomerBuilder().WithName(_customerName).Build());
            FlushAndClear();

            Assert.Throws<NHibernate.NonUniqueResultException>(() => _query.GetExecutableCriteria(Session).UniqueResult<Customer>());
        }
    }
}