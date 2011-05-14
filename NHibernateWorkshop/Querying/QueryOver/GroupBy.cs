using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Transform;
using Northwind.Dtos;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.QueryOver
{
    [TestFixture]
    public class GroupBy : AutoRollbackFixture
    {
        private IList<OrderItem> _orderItems;

        protected override void AfterSetUp()
        {
            _orderItems = Session.QueryOver<OrderItem>().List();
        }

        [Test]
        public void retrieve_product_sales_summaries()
        {
            Product product = null;

            var productSalesSummaries = Session.QueryOver<OrderItem>()
                .JoinAlias(o => o.Product, () => product)
                .Select(Projections.ProjectionList()
                    .Add(Projections.Group(() => product.Id).As("ProductId"))
                    .Add(Projections.Group(() => product.Name).As("ProductName"))
                    .Add(Projections.Sum<OrderItem>(o => o.Quantity).As("ItemsSold")))
                .TransformUsing(new AliasToBeanResultTransformer(typeof(ProductSalesSummary)))
                .List<ProductSalesSummary>();

            productSalesSummaries.Each(p => Assert.AreEqual(GetSalesCountForProductWithId(p.ProductId), p.ItemsSold));
        }

        [Test]
        public void retrieve_product_sales_summaries_of_products_that_sold_at_least_100_items()
        {
            Product product = null;

            var productSalesSummaries = Session.QueryOver<OrderItem>()
                .JoinAlias(o => o.Product, () => product)
                .Select(Projections.ProjectionList()
                    .Add(Projections.Group(() => product.Id).As("ProductId"))
                    .Add(Projections.Group(() => product.Name).As("ProductName"))
                    .Add(Projections.Sum<OrderItem>(o => o.Quantity).As("ItemsSold")))
                .Where(Restrictions.Gt(Projections.Sum<OrderItem>(o => o.Quantity), 100))
                .TransformUsing(new AliasToBeanResultTransformer(typeof(ProductSalesSummary)))
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