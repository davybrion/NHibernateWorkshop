using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Northwind.Entities;

namespace Northwind.FluentNHibernate.Overrides
{
    public class ThirdPartyMappingOverride : IAutoMappingOverride<ThirdParty>
    {
        public void Override(AutoMapping<ThirdParty> mapping)
        {
            // Table Per Hierarchy example
            mapping.DiscriminateSubClassesOnColumn("Type");
            mapping.SubClass<Customer>("Customer"); // the "Customer" value is ignored by FluentNHibernate, it uses the full typename by default
            mapping.SubClass<Supplier>("Supplier"); // the "Supplier" value is also ignored

            // Table Per SubClass example
            //mapping.JoinedSubClass<Customer>("Id", customerMap => customerMap.Map(c => c.DiscountPercentage).Not.Nullable());
            //mapping.JoinedSubClass<Supplier>("Id");

            // Table Per SubClass with discriminator can't be done with Fluent NHibernate (as far as i know)

            // Table per Concrete class example (union-subclass) doesn't seem to work with Fluent NHibernate and automapping yet either?

            mapping.Id(o => o.Id).GeneratedBy.HiLo("100");

            mapping.Map(t => t.Name)
                .Length(100)
                .Not.Nullable();

            mapping.Map(t => t.ContactName).Length(100);
            mapping.Map(t => t.Phone).Length(15);
            mapping.Map(t => t.Fax).Length(15);
        }
    }
}