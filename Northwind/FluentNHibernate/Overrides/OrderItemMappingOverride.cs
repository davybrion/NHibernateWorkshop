using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Northwind.Entities;

namespace Northwind.FluentNHibernate.Overrides
{
    public class OrderItemMappingOverride : IAutoMappingOverride<OrderItem>
    {
        public void Override(AutoMapping<OrderItem> mapping)
        {
            mapping.Id(o => o.Id).GeneratedBy.HiLo("100");

            mapping.References(o => o.Product).Not.Nullable();
            mapping.Map(o => o.UnitPrice).Not.Nullable();
            mapping.Map(o => o.Quantity).Not.Nullable();
        }
    }
}