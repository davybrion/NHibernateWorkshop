using NHibernate.Criterion;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.Criteria
{
    [TestFixture]
    public class DetachedCriteriaVsCriteria : Fixture
    {
        [Test]
        public void detached_criteria_can_be_created_outside_of_session_scope()
        {
            var query = DetachedCriteria.For<Customer>()
                .Add(Restrictions.Gt("DiscountPercentage", 0.2d));

            using (var session = CreateSession())
            {
                Logger.Info("the query has already been created but we can only execute it through an open session");
                var results = query.GetExecutableCriteria(session).List();
            }
        }
    }
}