using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.Criteria
{
    [TestFixture]
    public class Polymorphic : AutoRollbackFixture
    {
        [Test]
        public void selecting_all_third_parties_return_both_customers_and_suppliers()
        {
            var customers = Session.CreateCriteria<Customer>().List<Customer>();
            var suppliers = Session.CreateCriteria<Supplier>().List<Supplier>();
            var thirdParties = Session.CreateCriteria<ThirdParty>().List<ThirdParty>();

            customers.Each(c => Assert.IsTrue(thirdParties.Contains(c)));
            suppliers.Each(s => Assert.IsTrue(thirdParties.Contains(s)));
        }
    }
}