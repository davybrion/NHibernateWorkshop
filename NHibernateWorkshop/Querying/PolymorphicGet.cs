using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying
{
    [TestFixture]
    public class PolymorphicGet : AutoRollbackFixture
    {
        [Test]
        public void selecting_a_specific_instance_through_its_super_class_is_possible()
        {
            var customer = Session.CreateCriteria<Customer>().SetMaxResults(1).UniqueResult<Customer>();
            Session.Evict(customer);
            var thirdParty = Session.Get<ThirdParty>(customer.Id);
            // be sure to check the output of this test... the generated statement for the Get call
            // can vary quite a bit depending on which inheritance strategy you've chosen in the mappings
            Assert.AreEqual(customer, thirdParty);
        }
    }
}