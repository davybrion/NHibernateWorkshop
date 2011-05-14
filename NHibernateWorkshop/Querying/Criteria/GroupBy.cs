using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Transform;
using Northwind.Dtos;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.Criteria
{
    [TestFixture]
    public class GroupBy : AutoRollbackFixture
    {
        private IList<OrderItem> _orderItems;

        protected override void AfterSetUp()
        {
            _orderItems = Session.CreateCriteria<OrderItem>().List<OrderItem>();
        }

        [Test]
        public void retrieve_product_sales_summaries()
        {
            var productSalesSummaries = Session.CreateCriteria<OrderItem>()
                .CreateAlias("Product", "product")
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.GroupProperty("product.Id"), "ProductId")
                    .Add(Projections.GroupProperty("product.Name"), "ProductName")
                    .Add(Projections.Sum("Quantity"), "ItemsSold"))
                .SetResultTransformer(new AliasToBeanResultTransformer(typeof(ProductSalesSummary)))
                .List<ProductSalesSummary>();

            productSalesSummaries.Each(p => Assert.AreEqual(GetSalesCountForProductWithId(p.ProductId), p.ItemsSold));
        }

        [Test]
        public void retrieve_product_sales_summaries_of_products_that_sold_at_least_100_items()
        {
            var productSalesSummaries = Session.CreateCriteria<OrderItem>()
                .CreateAlias("Product", "product")
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.GroupProperty("product.Id"), "ProductId")
                    .Add(Projections.GroupProperty("product.Name"), "ProductName")
                    .Add(Projections.Sum("Quantity"), "ItemsSold"))
                .Add(Restrictions.Gt(Projections.Sum("Quantity"), 100))
                .SetResultTransformer(new AliasToBeanResultTransformer(typeof(ProductSalesSummary)))
                .List<ProductSalesSummary>();

            productSalesSummaries.Each(p =>
            {
                Assert.AreEqual(GetSalesCountForProductWithId(p.ProductId), p.ItemsSold);
                Assert.Greater(p.ItemsSold, 100);
            });
        }

        private long GetSalesCountForProductWithId(Guid productId)
        {
            return _orderItems.Where(o => o.Product.Id == productId).Sum(o => o.Quantity);
        }
    }
}