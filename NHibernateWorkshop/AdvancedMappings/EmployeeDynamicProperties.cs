using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.AdvancedMappings
{
    // Employee.DynamicProperties is mapped as a 'Map' of 'Elements'
    [TestFixture]
    public class EmployeeDynamicProperties : AutoRollbackFixture
    {
        private Employee _employee;

        protected override void AfterSetUp()
        {
            _employee = new EmployeeBuilder().Build();
            _employee.DynamicProperties["nationality"] = "Belgian";
            _employee.DynamicProperties["sex"] = "male";
            Session.Save(_employee);
            FlushAndClear();
        }

        [Test]
        public void added_dynamic_properties_are_persisted()
        {
            var retrievedEmployee = Session.Get<Employee>(_employee.Id);
            Assert.AreEqual("Belgian", retrievedEmployee.DynamicProperties["nationality"]);
            Assert.AreEqual("male", retrievedEmployee.DynamicProperties["sex"]);
        }

        [Test]
        public void dynamic_properties_can_be_updated()
        {
            var retrievedEmployee = Session.Get<Employee>(_employee.Id);
            retrievedEmployee.DynamicProperties["nationality"] = "Spanish";
            FlushAndClear();
            Assert.AreEqual("Spanish", Session.Get<Employee>(_employee.Id).DynamicProperties["nationality"]);
        }

        [Test]
        public void dynamic_properties_can_be_removed()
        {
            var retrievedEmployee = Session.Get<Employee>(_employee.Id);
            retrievedEmployee.DynamicProperties.Remove("nationality");
            FlushAndClear();
            Assert.IsFalse(Session.Get<Employee>(_employee.Id).DynamicProperties.ContainsKey("nationality"));
        }

        [Test]
        public void dynamic_properties_that_are_set_to_null_are_empty_properties_in_the_database()
        {
            var retrievedEmployee = Session.Get<Employee>(_employee.Id);
            retrievedEmployee.DynamicProperties["nationality"] = null;
            FlushAndClear();
            Assert.IsNull(Session.Get<Employee>(_employee.Id).DynamicProperties["nationality"]);
        }
    }
}