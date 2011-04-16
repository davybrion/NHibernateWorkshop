using System.Collections.Generic;
using System.Linq;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.HQL
{
    [TestFixture]
    public class Paging : AutoRollbackFixture
    {
        private IEnumerable<Employee> _employees;

        protected override void AfterSetUp()
        {
            _employees = Session.CreateQuery("from Employee").List<Employee>();
            Clear();
        }

        [Test]
        public void page_through_all_employees()
        {
            const int pageSize = 5;
            var count = _employees.Count();
            var pages = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;

            for (int currentPageIndex = 0; currentPageIndex < pages; currentPageIndex++)
            {
                Logger.Info(string.Format("retrieving page {0}", currentPageIndex + 1));
                var currentPageOfEmployees = Session.CreateQuery("from Employee")
                    .SetFirstResult(currentPageIndex * pageSize)
                    .SetMaxResults(pageSize)
                    .List<Employee>();

                var inMemoryPage = _employees.Skip(currentPageIndex * pageSize).Take(pageSize);

                Assert.AreEqual(inMemoryPage.Count(), currentPageOfEmployees.Count);

                for (int i = 0; i < currentPageOfEmployees.Count; i++)
                {
                    Assert.AreEqual(inMemoryPage.ElementAt(i), currentPageOfEmployees[i]);
                }
            }
        }
    }
}