using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Northwind.Entities;

namespace Northwind.FluentNHibernate.Overrides
{
    public class ThirdPartyMappingOverride : IAutoMappingOverride<ThirdParty>
    {
        public void Override(AutoMapping<ThirdParty> mapping)
        {
            mapping.DiscriminateSubClassesOnColumn("Type");
            // TODO: these values seem to get ignored? it uses the full type-names in the discriminator column
            mapping.SubClass<Customer>("Customer");
            mapping.SubClass<Supplier>("Supplier");

            mapping.Id(o => o.Id).GeneratedBy.HiLo("100");

            mapping.Map(t => t.Name)
                .Length(100)
                .Not.Nullable();

            mapping.Map(t => t.ContactName).Length(100);
            mapping.Map(t => t.Phone).Length(15);
            mapping.Map(t => t.Fax).Length(15);

            // TODO: find out wether the Address fields are generated as non-nullable
        }
    }
}