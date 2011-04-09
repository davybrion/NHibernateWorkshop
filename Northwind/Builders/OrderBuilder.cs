using System;
using Northwind.Components;
using Northwind.Entities;

namespace Northwind.Builders
{
    public class OrderBuilder
    {
        private Customer _customer = new CustomerBuilder().Build();
        private Employee _employee = new EmployeeBuilder().Build();
        private Address _deliveryAddress;
        private DateTime _orderedOn = DateTime.Now;
        private DateTime? _shippedOn;
        
        public OrderBuilder WithCustomer(Customer customer)
        {
            _customer = customer;
            return this;
        }

        public OrderBuilder WithEmployee(Employee employee)
        {
            _employee = employee;
            return this;
        }

        public OrderBuilder WithDeliveryAddress(Address address)
        {
            _deliveryAddress = address;
            return this;
        }

        public OrderBuilder OrderedOn(DateTime orderedOn)
        {
            _orderedOn = orderedOn;
            return this;
        }

        public OrderBuilder ShippedOn(DateTime shippedOn)
        {
            _shippedOn = shippedOn;
            return this;
        }

        public Order Build()
        {
            var order = new Order(_customer, _employee, _orderedOn);

            if (_deliveryAddress != null) order.DeliveryAddress = _deliveryAddress;
            if (_shippedOn.HasValue) order.ShippedOn = _shippedOn.Value;

            return order;
        }
    }
}