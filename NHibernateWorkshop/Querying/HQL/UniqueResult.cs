using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.HQL
{
    [TestFixture]
    public class UniqueResult : AutoRollbackFixture
    {
        private const string _customerName = "TestCustomer";

        [Test]
        public void uniqueresult_returns_one_value_instead_of_list_with_one_value()
        {
            Session.Save(new CustomerBuilder().WithName(_customerName).Build());
            FlushAndClear();

            Logger.Info("The following call executes the query and returns a list of values, even though there's only one");
            var nameThroughList = Session.CreateQuery("from Customer c where c.Name = :name")
                .SetParameter("name", _customerName)
                .List<Customer>()[0].Name;
            Assert.AreEqual(_customerName, nameThroughList);

            Logger.Info("Calling UniqueResult instead of list returns just one value, even though the select statement will be the same");
            var nameThroughUniqueResult = Session.CreateQuery("from Customer c where c.Name = :name")
                .SetParameter("name", _customerName)
                .UniqueResult<Customer>().Name;
            Assert.AreEqual(_customerName, nameThroughUniqueResult);
        }

        [Test]
        public void uniqueresult_fails_when_resultset_contains_more_than_one_row()
        {
            Session.Save(new CustomerBuilder().WithName(_customerName).Build());
            Session.Save(new CustomerBuilder().WithName(_customerName).Build());
            FlushAndClear();

            Assert.Throws<NHibernate.NonUniqueResultException>(
                () => Session.CreateQuery("from Customer c where c.Name = :name")
                          .SetParameter("name", _customerName)
                          .UniqueResult<Customer>());
        }
    }
}