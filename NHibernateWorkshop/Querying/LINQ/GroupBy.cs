using System.Collections.Generic;
using System.Linq;
using NHibernate.Transform;
using NHibernate.Linq;
using Northwind.Dtos;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Querying.LINQ
{
    [TestFixture]
    public class GroupBy : AutoRollbackFixture
    {
        private IList<OrderItem> _orderItems;

        protected override void AfterSetUp()
        {
            _orderItems = Session.Query<OrderItem>().ToList();
        }

        [Test]
        public void retrieve_product_sales_summaries()
        {
            var productSalesSummaries = from orderItem in Session.Query<OrderItem>()
                                        group orderItem by new { orderItem.Product.Id, orderItem.Product.Name } into grouping
                                        select new { ProductId = grouping.Key.Id, ProductName = grouping.Key.Name, ItemsSold = grouping.Sum(o => o.Quantity) } ;

            // if i use the following select instead of using the anonymous type, NHibernate throws a casting exception for some unclear reason
            // select new ProductSalesSummary(grouping.Key.Id, grouping.Key.Name, grouping.Sum(o => o.Quantity));

            productSalesSummaries.Each(p => Assert.AreEqual(GetSalesCountForProductWithId(p.ProductId), p.ItemsSold));
        }
        
        // NOTE: i can't get this working with Linq and NHibernate :s

        //[Test]
        //public void retrieve_product_sales_summaries_of_products_that_sold_at_least_100_items()
        //{
        //    var productSalesSummaries = from orderItem in Session.Query<OrderItem>()
        //                                group orderItem by new { orderItem.Product.Id, orderItem.Product.Name } into grouping
        //                                where grouping.Sum(o => o.Quantity) > 100
        //                                select new { ProductId = grouping.Key.Id, ProductName = grouping.Key.Name, ItemsSold = grouping.Sum(o => o.Quantity) };

        //    productSalesSummaries.Each(p =>
        //    {
        //        Assert.AreEqual(GetSalesCountForProductWithId(p.ProductId), p.ItemsSold);
        //        Assert.Greater(p.ItemsSold, 100);
        //    });
        //}

        private long GetSalesCountForProductWithId(int productId)
        {
            return _orderItems.Where(o => o.Product.Id == productId).Sum(o => o.Quantity);
        }
    }
}