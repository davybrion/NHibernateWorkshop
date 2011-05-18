using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Northwind.Entities;

namespace Northwind.FluentNHibernate.Overrides
{
    public class UserGroupMappingOverride : IAutoMappingOverride<UserGroup>
    {
        public void Override(AutoMapping<UserGroup> mapping)
        {
            mapping.Map(u => u.Name)
                .Not.Nullable()
                .Length(20);
        }
    }
}