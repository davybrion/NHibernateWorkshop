using Northwind.Entities;
using Northwind.Enums;

namespace Northwind.Builders
{
    public class ProductBuilder
    {
        private string _name = "Jupiler";
        private ProductCategory _category = ProductCategory.Beverages;
        private double? _unitPrice;
        private int? _unitsInStock;
        private int? _unitsOnOrder;
        private int? _reorderLevel;
        private bool? _discontinued;

        public ProductBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public ProductBuilder WithCategory(ProductCategory productCategory)
        {
            _category = productCategory;
            return this;
        }

        public ProductBuilder WithUnitPrice(double unitPrice)
        {
            _unitPrice = unitPrice;
            return this;
        }

        public ProductBuilder WithUnitsInStock(int unitsInStock)
        {
            _unitsInStock = unitsInStock;
            return this;
        }

        public ProductBuilder WithUnitsOnOrder(int unitsOnOrder)
        {
            _unitsOnOrder = unitsOnOrder;
            return this;
        }

        public ProductBuilder WithReorderLevel(int reorderLevel)
        {
            _reorderLevel = reorderLevel;
            return this;
        }

        public ProductBuilder IsDiscontinued()
        {
            _discontinued = true;
            return this;
        }

        public Product Build()
        {
            var product = new Product(_name, _category);

            if (_unitPrice.HasValue) product.UnitPrice = _unitPrice.Value;
            if (_unitsInStock.HasValue) product.UnitsInStock = _unitsInStock.Value;
            if (_unitsOnOrder.HasValue) product.UnitsOnOrder = _unitsOnOrder.Value;
            if (_reorderLevel.HasValue) product.ReorderLevel = _reorderLevel.Value;
            if (_discontinued.HasValue) product.Discontinued = _discontinued.Value;

            return product;
        }
    }
}