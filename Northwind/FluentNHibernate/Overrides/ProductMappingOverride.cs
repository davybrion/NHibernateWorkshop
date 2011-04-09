using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using Northwind.Entities;

namespace Northwind.FluentNHibernate.Overrides
{
    public class ProductMappingOverride : IAutoMappingOverride<Product>
    {
        public void Override(AutoMapping<Product> mapping)
        {
            mapping.Version(p => p.Version);

            mapping.Map(p => p.Name)
                .Length(50)
                .Not.Nullable();

            mapping.Map(p => p.Category)
                .Not.Nullable();

            mapping.HasMany(p => p.Sources)
                .AsBag()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .ExtraLazyLoad()
                .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}