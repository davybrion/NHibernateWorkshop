using NHibernate.Criterion;
using NHibernate.Linq;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace NHibernateWorkshop.Querying.LINQ
{
    [TestFixture]
    public class Restricting : AutoRollbackFixture
    {
        private Product _product1;
        private Product _product2;
        private Product _product3;

        protected override void AfterSetUp()
        {
            _product1 = new ProductBuilder().WithName("product 1").Build();
            _product2 = new ProductBuilder().WithName("product 2").Build();
            _product3 = new ProductBuilder().WithName("product 3").Build();
            Session.Save(_product1);
            Session.Save(_product2);
            Session.Save(_product3);
            Flush();
        }

        [Test]
        public void where_properties_are_equal()
        {
            _product1.UnitsInStock = 10;
            _product1.UnitsOnOrder = 10;
            _product2.UnitsInStock = 13;
            _product2.UnitsOnOrder = 14;
            _product3.UnitsInStock = 7;
            _product3.UnitsOnOrder = 7;
            Flush();

            var products = Session.Query<Product>()
                .Where(p => p.UnitsInStock == p.UnitsOnOrder)
                .ToList();

            Assert.IsTrue(products.Contains(_product1));
            Assert.IsFalse(products.Contains(_product2));
            Assert.IsTrue(products.Contains(_product3));
            products.Each(p => Assert.AreEqual(p.UnitsInStock, p.UnitsOnOrder));
        }

        [Test]
        public void where_one_property_is_greater_than_the_other()
        {
            _product1.UnitsInStock = 10;
            _product1.UnitsOnOrder = 10;
            _product2.UnitsInStock = 13;
            _product2.UnitsOnOrder = 14;
            _product3.UnitsInStock = 2;
            _product3.UnitsOnOrder = 7;
            Flush();

            var products = Session.Query<Product>()
                .Where(p => p.UnitsOnOrder > p.UnitsInStock)
                .ToList();

            Assert.IsFalse(products.Contains(_product1));
            Assert.IsTrue(products.Contains(_product2));
            Assert.IsTrue(products.Contains(_product3));
            products.Each(p => Assert.Greater(p.UnitsOnOrder, p.UnitsInStock));
        }

        [Test]
        public void where_one_property_has_a_value_between_a_given_range()
        {
            _product1.UnitsInStock = 10;
            _product2.UnitsInStock = 13;
            _product3.UnitsInStock = 9;
            Flush();

            var products = Session.Query<Product>()
                //.Where(p => p.UnitsInStock.IsBetween(5).And(10)) // this unfortunately throws an exception while generating the query :s
                .Where(p => p.UnitsInStock >= 5 && p.UnitsInStock <= 10) // no way to use between in generated sql as far as i know
                .ToList();

            Assert.IsTrue(products.Contains(_product1));
            Assert.IsFalse(products.Contains(_product2));
            Assert.IsTrue(products.Contains(_product3));
            products.Each(p => Assert.IsTrue(p.UnitsInStock >= 5 && p.UnitsInStock <= 10));
        }

        [Test]
        public void where_one_property_has_a_value_in_a_given_set()
        {
            _product1.UnitsInStock = 10;
            _product2.UnitsInStock = 13;
            _product3.UnitsInStock = 9;
            Flush();

            var stockLevels = new List<int>() {7, 9, 11, 13}; 
            var products = Session.Query<Product>()
                .Where(p => stockLevels.Contains(p.UnitsInStock.Value)) // this only works with concrete List objects
                .ToList();

            Assert.IsFalse(products.Contains(_product1));
            Assert.IsTrue(products.Contains(_product2));
            Assert.IsTrue(products.Contains(_product3));
            products.Each(p => Assert.IsTrue(stockLevels.Contains(p.UnitsInStock.Value)));
        }

        [Test]
        public void where_string_property_contains_value()
        {
            var products = Session.Query<Product>()
                .Where(p => p.Name.Contains("roduc"))
                .ToList();

            Assert.IsTrue(products.Contains(_product1));
            Assert.IsTrue(products.Contains(_product2));
            Assert.IsTrue(products.Contains(_product3));
            products.Each(p => Assert.IsTrue(p.Name.Contains("roduc")));
        }

        [Test]
        public void where_string_property_begins_with_value()
        {
            var products = Session.Query<Product>()
                .Where(p => p.Name.StartsWith("pr"))
                .ToList();

            Assert.IsTrue(products.Contains(_product1));
            Assert.IsTrue(products.Contains(_product2));
            Assert.IsTrue(products.Contains(_product3));
            products.Each(p => Assert.IsTrue(p.Name.StartsWith("pr")));
        }

        [Test]
        public void where_string_property_ends_with_value()
        {
            var products = Session.Query<Product>()
                .Where(p => p.Name.EndsWith("3"))
                .ToList();

            Assert.IsFalse(products.Contains(_product1));
            Assert.IsFalse(products.Contains(_product2));
            Assert.IsTrue(products.Contains(_product3));
            products.Each(p => Assert.IsTrue(p.Name.EndsWith("3")));
        }

        [Test]
        public void where_string_property_matches_value_exactly()
        {
            var product4 = new ProductBuilder().WithName("blah product 2 blah").Build();
            Session.Save(product4);
            Flush();

            var products = Session.Query<Product>()
                .Where(p => p.Name == "product 2")
                .ToList();

            Assert.IsFalse(products.Contains(_product1));
            Assert.IsTrue(products.Contains(_product2));
            Assert.IsFalse(products.Contains(_product3));
            Assert.IsFalse(products.Contains(product4));
            products.Each(p => Assert.AreEqual("product 2", p.Name));
        }

        [Test]
        public void where_property_is_null()
        {
            var products = Session.Query<Product>()
                .Where(p => p.ReorderLevel == null)
                .ToList();

            Assert.IsTrue(products.Contains(_product1));
            Assert.IsTrue(products.Contains(_product2));
            Assert.IsTrue(products.Contains(_product3));
            products.Each(p => Assert.IsNull(p.ReorderLevel));
        }

        [Test]
        public void where_property_is_not_null()
        {
            var products = Session.Query<Product>()
                .Where(p => p.ReorderLevel != null)
                .ToList();

            Assert.IsFalse(products.Contains(_product1));
            Assert.IsFalse(products.Contains(_product2));
            Assert.IsFalse(products.Contains(_product3));
            products.Each(p => Assert.IsNotNull(p.ReorderLevel));
        }

        [Test]
        public void where_collection_is_empty()
        {
            var products = Session.Query<Product>()
                .Where(p => p.Sources.Count() == 0)
                .ToList();

            Assert.IsTrue(products.Contains(_product1));
            Assert.IsTrue(products.Contains(_product2));
            Assert.IsTrue(products.Contains(_product3));
            products.Each(p => Assert.AreEqual(0, p.Sources.Count()));
        }

        [Test]
        public void where_collection_is_not_empty()
        {
            var products = Session.Query<Product>()
                .Where(p => p.Sources.Count() > 0)
                .ToList();

            Assert.IsFalse(products.Contains(_product1));
            Assert.IsFalse(products.Contains(_product2));
            Assert.IsFalse(products.Contains(_product3));
            products.Each(p => Assert.Greater(p.Sources.Count(), 0));
        }

        [Test]
        public void two_restrictions_with_and()
        {
            _product1.AddSource(new SupplierBuilder().Build(), 10);
            _product1.UnitsInStock = 5;
            Flush();

            var products = Session.Query<Product>()
                .Where(p => p.Sources.Count() > 0)
                .Where(p => p.UnitsInStock != null)
                .ToList();

            Assert.IsTrue(products.Contains(_product1));
            Assert.IsFalse(products.Contains(_product2));
            Assert.IsFalse(products.Contains(_product3));
            products.Each(p =>
            {
                Assert.Greater(p.Sources.Count(), 0);
                Assert.IsNotNull(p.UnitsInStock);
            });
        }

        [Test]
        public void two_restrictions_with_or()
        {
            var products = Session.Query<Product>()
                .Where(p => p.Sources.Count() == 0 || p.UnitsInStock == null)
                .ToList();

            Assert.IsTrue(products.Contains(_product1));
            Assert.IsTrue(products.Contains(_product2));
            Assert.IsTrue(products.Contains(_product3));
            products.Each(p => Assert.IsTrue(p.Sources.Count() == 0 || p.UnitsInStock == null));
        }

        [Test]
        public void more_than_two_restrictions_with_and()
        {
            _product1.AddSource(new SupplierBuilder().Build(), 10);
            _product1.UnitsInStock = 5;
            _product3.ReorderLevel = null;
            Flush();

            var products = Session.Query<Product>()
                .Where(p => p.Sources.Count() > 0)
                .Where(p => p.UnitsInStock != null)
                .Where(p => p.ReorderLevel == null)
                .ToList();

            Assert.IsTrue(products.Contains(_product1));
            Assert.IsFalse(products.Contains(_product2));
            Assert.IsFalse(products.Contains(_product3));
            products.Each(p =>
            {
                Assert.Greater(p.Sources.Count(), 0);
                Assert.IsNotNull(p.UnitsInStock);
                Assert.IsNull(p.ReorderLevel);
            });
        }

        [Test]
        public void more_than_two_restrictions_with_or()
        {
            var products = Session.Query<Product>()
                .Where(p => p.Sources.Count() == 0 || p.UnitsInStock == null || p.ReorderLevel != null)
                .ToList();

            Assert.IsTrue(products.Contains(_product1));
            Assert.IsTrue(products.Contains(_product2));
            Assert.IsTrue(products.Contains(_product3));
            products.Each(p => Assert.IsTrue(p.Sources.Count() == 0 || p.UnitsInStock == null || p.ReorderLevel.HasValue));
        }
    }
}