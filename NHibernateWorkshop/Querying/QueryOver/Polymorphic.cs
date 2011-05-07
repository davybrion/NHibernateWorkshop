using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.QueryOver
{
    [TestFixture]
    public class Polymorphic : AutoRollbackFixture
    {
        [Test]
        public void selecting_all_third_parties_return_both_customers_and_suppliers()
        {
            var customers = Session.QueryOver<Customer>().List();
            var suppliers = Session.QueryOver<Supplier>().List();
            var thirdParties = Session.QueryOver<ThirdParty>().List();

            customers.Each(c => Assert.IsTrue(thirdParties.Contains(c)));
            suppliers.Each(s => Assert.IsTrue(thirdParties.Contains(s)));
        }
    }
}