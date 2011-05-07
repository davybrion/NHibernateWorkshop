using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Transform;
using Northwind.Dtos;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.HQL
{
    [TestFixture]
    public class GroupBy : AutoRollbackFixture
    {
        private IList<OrderItem> _orderItems;

        protected override void AfterSetUp()
        {
            _orderItems = Session.CreateQuery("from OrderItem").List<OrderItem>();
        }

        [Test]
        public void retrieve_product_sales_summaries()
        {
            var productSalesSummaries =
                Session.CreateQuery(
                    @"select o.Product.Id as ProductId, o.Product.Name as ProductName, sum(o.Quantity) as ItemsSold
                        from OrderItem o group by o.Product, o.Product.Name")
                    .SetResultTransformer(new AliasToBeanResultTransformer(typeof(ProductSalesSummary)))
                    .List<ProductSalesSummary>();

            productSalesSummaries.Each(p => Assert.AreEqual(GetSalesCountForProductWithId(p.ProductId), p.ItemsSold));
        }
        
        [Test]
        public void retrieve_product_sales_summaries_of_products_that_sold_at_least_100_items()
        {
            var productSalesSummaries =
                Session.CreateQuery(
                    @"select o.Product.Id as ProductId, o.Product.Name as ProductName, sum(o.Quantity) as ItemsSold
                        from OrderItem o group by o.Product, o.Product.Name
                        having sum(o.Quantity) > 100")
                    .SetResultTransformer(new AliasToBeanResultTransformer(typeof(ProductSalesSummary)))
                    .List<ProductSalesSummary>();

            productSalesSummaries.Each(p =>
            {
                Assert.AreEqual(GetSalesCountForProductWithId(p.ProductId), p.ItemsSold);
                Assert.Greater(p.ItemsSold, 100);
            });
        }

        private long GetSalesCountForProductWithId(int productId)
        {
            return _orderItems.Where(o => o.Product.Id == productId).Sum(o => o.Quantity);
        }
    }
}