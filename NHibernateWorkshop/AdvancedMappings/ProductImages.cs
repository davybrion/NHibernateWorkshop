using Northwind.Builders;
using Northwind.Components;
using Northwind.Entities;
using Northwind.Enums;
using NUnit.Framework;

namespace NHibernateWorkshop.AdvancedMappings
{
    // Product.Images is mapped as a bag of components
    [TestFixture]
    public class ProductImages : AutoRollbackFixture
    {
        private Product _product;
        private ImageInfo _image1;
        private ImageInfo _image2;

        protected override void AfterSetUp()
        {
            _product = new ProductBuilder().Build();
            _image1 = new ImageInfo("1.png", ImageType.Png);
            _image2 = new ImageInfo("2.jpg", ImageType.Jpeg);
            _product.Images.Add(_image1);
            _product.Images.Add(_image2);
            Session.Save(_product);
        }

        [Test]
        public void images_can_be_added_to_a_product()
        {
            FlushAndClear();

            var retrievedProduct = Session.Get<Product>(_product.Id);
            Assert.IsTrue(retrievedProduct.Images.Contains(_image1));
            Assert.IsTrue(retrievedProduct.Images.Contains(_image2));
        }

        [Test]
        public void images_can_be_removed_from_a_product()
        {
            Flush();
            _product.Images.Remove(_image1);
            FlushAndClear();

            var retrievedProduct = Session.Get<Product>(_product.Id);
            Assert.IsFalse(retrievedProduct.Images.Contains(_image1));
            Assert.IsTrue(retrievedProduct.Images.Contains(_image2));
        }
    }
}