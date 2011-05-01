using System.Collections.Generic;
using System.Linq;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Performance
{
    [TestFixture]
    public class ExtraLazyCollection : AutoRollbackFixture
    {
        private int _productId;
        private IEnumerable<ProductSource> _sources;

        protected override void AfterSetUp()
        {
            _productId = Session.CreateQuery("select p.Id from Product p where exists elements(p.Sources)")
                .SetMaxResults(1)
                .UniqueResult<int>();

            _sources = Session.CreateQuery("from ProductSource ps where ps.Product.Id = :id")
                .SetParameter("id", _productId)
                .List<ProductSource>();

            Clear();
        }

        [Test]
        public void retrieving_count_of_sources_issues_only_a_count_statement()
        {
            var product = Session.Get<Product>(_productId);
            Logger.Info("about to retrieve the number of sources");
            var count = product.Sources.Count();
            Logger.Info("there should be just one count statement and no select of the entire collection");
        }

        [Test]
        public void contains_issues_single_exists_statement()
        {
            var product = Session.Get<Product>(_productId);
            Logger.Info("about to check whether our Sources collection contains one of the known ProductSources");
            Assert.IsTrue(product.Sources.Contains(_sources.ElementAt(0)));
            Logger.Info("there should be just one select 1 statement and no select of the entire collection");
        }
    }
}