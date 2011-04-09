using System;
using Northwind.Components;
using Northwind.Entities;

namespace Northwind.Builders
{
    public class EmployeeBuilder
    {
        private string _firstName = "Jim";
        private string _lastName = "O'Donnel";
        private Address _address = new Address("some street", "some city", 45634, "some country");
        private DateTime _birthDate = new DateTime(1981, 6, 19);
        private DateTime _hireDate = new DateTime(2008, 3, 1);
        private string _title;
        private string _phone;
        private Employee _manager;

        public EmployeeBuilder WithFirstName(string firstName)
        {
            _firstName = firstName;
            return this;
        }

        public EmployeeBuilder WithLastName(string lastName)
        {
            _lastName = lastName;
            return this;
        }

        public EmployeeBuilder WithAddress(Address address)
        {
            _address = address;
            return this;
        }

        public EmployeeBuilder WithBirthDate(DateTime birthDate)
        {
            _birthDate = birthDate;
            return this;
        }

        public EmployeeBuilder WithHireDate(DateTime hireDate)
        {
            _hireDate = hireDate;
            return this;
        }

        public EmployeeBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public EmployeeBuilder WithPhone(string phone)
        {
            _phone = phone;
            return this;
        }

        public EmployeeBuilder WithManager(Employee manager)
        {
            _manager = manager;
            return this;
        }

        public Employee Build()
        {
            var employee = new Employee(_firstName, _lastName, _address, _birthDate, _hireDate);

            if (_title != null) employee.Title = _title;
            if (_phone != null) employee.Phone = _phone;
            if (_manager != null) employee.Manager = _manager;

            return employee;
        }
    }
}