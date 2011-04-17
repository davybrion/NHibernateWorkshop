using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.QueryOver
{
    [TestFixture]
    public class DetachedQueryOverVsQueryOver : Fixture
    {
        [Test]
        public void detached_queryover_can_be_created_outside_of_session_scope()
        {
            var query = NHibernate.Criterion.QueryOver.Of<Customer>()
                .Where(c => c.DiscountPercentage == 0.2d);

            using (var session = CreateSession())
            {
                Logger.Info("the query has already been created but we can only execute it through an open session");
                var results = query.GetExecutableQueryOver(session).List();
            }
        }
    }
}