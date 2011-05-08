using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.Components;

namespace Northwind.Entities
{
    public class Employee : Entity<int>
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Title { get; set; }
        public virtual DateTime BirthDate { get; set; }
        public virtual DateTime HireDate { get; set; }
        public virtual Address Address { get; set; }
        public virtual string Phone { get; set; }
        public virtual Employee Manager { get; set; }
        public virtual decimal Salary { get; set; }

        private IList<Employee> _subordinates = new List<Employee>();

        protected Employee() {}

        public Employee(string firstName, string lastName, Address address, DateTime birthDate, DateTime hireDate)
        {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            BirthDate = birthDate;
            HireDate = hireDate;
        }

        public virtual IEnumerable<Employee> Subordinates
        {
            get { return _subordinates.ToArray(); }
        }

        public virtual void AddSubordinate(Employee subordinate)
        {
            _subordinates.Add(subordinate);
            subordinate.Manager = this;
        }

        public virtual void RemoveSubordinate(Employee subordinate)
        {
            _subordinates.Remove(subordinate);
            subordinate.Manager = null;
        }
    }
}