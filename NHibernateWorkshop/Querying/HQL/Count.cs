using NUnit.Framework;

namespace NHibernateWorkshop.Querying.HQL
{
    [TestFixture]
    public class Count : AutoRollbackFixture
    {
        [Test]
        public void selecting_count_from_resultset_returns_size_of_resultset()
        {
            var directCount = Session.CreateQuery("select count(c) from Customer c where c.DiscountPercentage = :discount")
                .SetParameter<double>("discount", 0.2d)
                .UniqueResult();

            var calculatedCount = Session.CreateQuery("from Customer c where c.DiscountPercentage = :discount")
                .SetParameter("discount", 0.2d)
                .List().Count;

            Assert.AreEqual(calculatedCount, directCount);
        }
    }
}