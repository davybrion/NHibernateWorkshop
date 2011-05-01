using System;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;
using NHibernate.Linq;
using System.Linq;

namespace NHibernateWorkshop.Querying.LINQ
{
    [TestFixture]
    public class UniqueResult : AutoRollbackFixture
    {
        private const string _customerName = "TestCustomer";

        private IQueryable<Customer> _query;

        protected override void AfterSetUp()
        {
            _query = Session.Query<Customer>().Where(c => c.Name == _customerName);
        }

        [Test]
        public void single_returns_one_value_instead_of_list_with_one_value()
        {
            Session.Save(new CustomerBuilder().WithName(_customerName).Build());
            FlushAndClear();

            Logger.Info("The following call executes the query and returns a list of values, even though there's only one");
            Assert.AreEqual(_customerName, _query.ToList()[0].Name);
            Logger.Info("Calling SingleOrDefault instead of list returns just one value, even though the select statement will be the same");
            Assert.AreEqual(_customerName, _query.Single().Name);
        }

        [Test]
        public void single_fails_when_resultset_contains_more_than_one_row()
        {
            Session.Save(new CustomerBuilder().WithName(_customerName).Build());
            Session.Save(new CustomerBuilder().WithName(_customerName).Build());
            FlushAndClear();

            // note that with the linq implementation, it doesn't throw an NHibernate.NonUniqueResultException
            // like it does for the other querying implementations
            Assert.Throws<InvalidOperationException>(() => _query.Single());
        }
    }
}