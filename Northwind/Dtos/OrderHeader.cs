using System;

namespace Northwind.Dtos
{
    public class OrderHeader
    {
        public DateTime OrderedOn { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }

        protected OrderHeader() {}

        public OrderHeader(DateTime orderedOn, string customerName, string employeeName)
        {
            OrderedOn = orderedOn;
            CustomerName = customerName;
            EmployeeName = employeeName;
        }
    }
}