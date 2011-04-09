using System;
using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;
using Northwind.Components;

namespace Northwind.Entities
{
    public class Order : Entity<Guid>
    {
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual DateTime OrderedOn { get; set; }
        public virtual DateTime? ShippedOn { get; set; }
        public virtual Address DeliveryAddress { get; set; }

        private ISet<OrderItem> _items = new HashedSet<OrderItem>();

        protected Order() {}

        public Order(Customer customer, Employee employee) : this(customer, employee, DateTime.Now) {}

        public Order(Customer customer, Employee employee, DateTime orderedOn)
        {
            Customer = customer;
            Employee = employee;
            OrderedOn = orderedOn;
        }

        public virtual IEnumerable<OrderItem> Items
        {
            get { return _items.ToArray(); }
        }

        public virtual void AddItem(OrderItem item)
        {
            _items.Add(item);
        }

        public virtual void RemoveItem(OrderItem item)
        {
            _items.Remove(item);
        }
    }
}