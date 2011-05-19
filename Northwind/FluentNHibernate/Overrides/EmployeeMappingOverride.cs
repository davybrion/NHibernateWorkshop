using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using Northwind.Entities;

namespace Northwind.FluentNHibernate.Overrides
{
    public class EmployeeMappingOverride : IAutoMappingOverride<Employee>
    {
        public void Override(AutoMapping<Employee> mapping)
        {
            mapping.DynamicUpdate();
            mapping.OptimisticLock.Dirty();

            mapping.Map(e => e.FirstName)
                .Length(50)
                .Not.Nullable();

            mapping.Map(e => e.LastName)
                .Length(75)
                .Not.Nullable();

            mapping.Map(e => e.Title)
                .Length(100);

            mapping.Map(e => e.BirthDate)
                .Not.Nullable();

            mapping.Map(e => e.HireDate)
                .Not.Nullable();

            mapping.Map(e => e.Phone)
                .Length(15);

            mapping.HasMany(e => e.Subordinates)
                .AsBag()
                .Inverse()
                .KeyColumn("ManagerId")
                .Cascade.SaveUpdate()
                .Access.CamelCaseField(Prefix.Underscore);

            mapping.HasMany(e => e.DynamicProperties)
                .AsMap<string>("PropertyName")
                .Table("DynamicEmployeeProperties")
                .KeyColumn("EmployeeId")
                .Element("PropertyValue");

            mapping.HasOne(e => e.User)
                .Cascade.All();

            //mapping.HasOne(e => e.User)
            //    .Cascade.All()
            //    .PropertyRef(u => u.Employee);
        }
    }
}