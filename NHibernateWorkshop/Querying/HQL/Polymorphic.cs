using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.HQL
{
    [TestFixture]
    public class Polymorphic : AutoRollbackFixture
    {
        [Test]
        public void selecting_all_third_parties_return_both_customers_and_suppliers()
        {
            var customers = Session.CreateQuery("from Customer").List<Customer>();
            var suppliers = Session.CreateQuery("from Supplier").List<Supplier>();
            var thirdParties = Session.CreateQuery("from ThirdParty").List<ThirdParty>();

            customers.Each(c => Assert.IsTrue(thirdParties.Contains(c)));
            suppliers.Each(s => Assert.IsTrue(thirdParties.Contains(s)));
        }
    }
}