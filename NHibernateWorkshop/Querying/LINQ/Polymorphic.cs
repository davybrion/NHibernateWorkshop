using System.Linq;
using Northwind.Entities;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernateWorkshop.Querying.LINQ
{
    [TestFixture]
    public class Polymorphic : AutoRollbackFixture
    {
        [Test]
        public void selecting_all_third_parties_return_both_customers_and_suppliers()
        {
            var customers = Session.Query<Customer>().ToList();
            var suppliers = Session.Query<Supplier>().ToList();
            var thirdParties = Session.Query<ThirdParty>().ToList();

            customers.Each(c => Assert.IsTrue(thirdParties.Contains(c)));
            suppliers.Each(s => Assert.IsTrue(thirdParties.Contains(s)));
        }
    }
}