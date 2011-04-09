using System;

namespace Northwind.Entities
{
    public class OrderItem : Entity<int> // TODO: use HILO for this one
    {
        public virtual Product Product { get; private set; }
        public virtual double UnitPrice { get; set; }
        public virtual int Quantity { get; set; }
        public virtual double? DiscountPercentage { get; set; }

        protected OrderItem() {}

        public OrderItem(Product product, double unitPrice, int quantity)
        {
            Product = product;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}