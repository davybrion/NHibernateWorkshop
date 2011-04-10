using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Northwind.Entities;

namespace Northwind.FluentNHibernate.Overrides
{
    public class ProductSourceMappingOverride : IAutoMappingOverride<ProductSource>
    {
        public void Override(AutoMapping<ProductSource> mapping)
        {
            mapping.Id(p => p.Id).GeneratedBy.GuidComb();

            mapping.References(p => p.Product)
                .Not.Nullable();

            mapping.References(p => p.Supplier)
                .Not.Nullable()
                .Cascade.SaveUpdate();

            mapping.Map(p => p.Cost)
                .Not.Nullable();
        }
    }
}