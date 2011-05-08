using Northwind.Entities;
using Northwind.Enums;
using NUnit.Framework;
using QuickGenerate;
using QuickGenerate.Primitives;

namespace NHibernateWorkshop.Performance
{
    [TestFixture]
    public class PagingThroughCollection : AutoRollbackFixture
    {
        [Test]
        public void paging_through_collection()
        {
            var suppliers = Session.QueryOver<Supplier>().List();

            var productWith20Sources = new DomainGenerator()
                .With<ProductSource>(options => options.Ignore(productsource => productsource.Id))
                .ForEach<ProductSource>(productsource => Session.Save(productsource))
                .With<Product>(options => options.Ignore(product => product.Id))
                .With<Product>(options => options.Ignore(product => product.Version))
                .With<Product>(options => options.For(product => product.Category, ProductCategory.Beverages))
                .With<Product>(g => g.Method<double>(20 , 20, (product, d) => product.AddSource(suppliers.PickOne(), d)))
                .With<Product>(options => options.For(product => product.Name, new StringGenerator(1, 50)))
                .ForEach<Product>(product => Session.Save(product))
                .One<Product>();

            FlushAndClear();

            var retrievedProduct = Session.Get<Product>(productWith20Sources.Id);
            var numberOfSources = Session.CreateQuery("select size(p.Sources) from Product p where p.Id = :productId")
                .SetParameter("productId", retrievedProduct.Id)
                .UniqueResult<int>();

            const int pageSize = 5;
            var pages = numberOfSources % pageSize == 0 ? numberOfSources / pageSize : numberOfSources / pageSize + 1;

            for (int currentPageIndex = 0; currentPageIndex < pages; currentPageIndex++)
            {
                Logger.Info(string.Format("retrieving page {0}", currentPageIndex + 1));
                Session.CreateFilter(retrievedProduct.Sources, "")
                    .SetFirstResult(currentPageIndex * pageSize)
                    .SetMaxResults(pageSize)
                    .List<ProductSource>();
            }
        }
    }
}