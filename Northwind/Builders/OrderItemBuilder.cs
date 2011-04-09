using Northwind.Entities;

namespace Northwind.Builders
{
    public class OrderItemBuilder
    {
        private Product _product = new ProductBuilder().Build();
        private double _unitPrice = 0.70d;
        private int _quantity = 24;
        private double? _discountPercentage;

        public OrderItemBuilder WithProduct(Product product)
        {
            _product = product;
            return this;
        }

        public OrderItemBuilder WithUnitPrice(double unitPrice)
        {
            _unitPrice = unitPrice;
            return this;
        }

        public OrderItemBuilder WithQuantity(int quantity)
        {
            _quantity = quantity;
            return this;
        }

        public OrderItemBuilder WithDiscountPercentage(double discountPercentage)
        {
            _discountPercentage = discountPercentage;
            return this;
        }

        public OrderItem Build()
        {
            var orderItem = new OrderItem(_product, _unitPrice, _quantity);

            if (_discountPercentage.HasValue) orderItem.DiscountPercentage = _discountPercentage.Value;

            return orderItem;
        }
    }
}