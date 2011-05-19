using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using Northwind.Entities;

namespace Northwind.FluentNHibernate.Overrides
{
    public class UserGroupMappingOverride : IAutoMappingOverride<UserGroup>
    {
        public void Override(AutoMapping<UserGroup> mapping)
        {
            mapping.Cache.ReadWrite();

            mapping.Id(o => o.Id).GeneratedBy.HiLo("100");

            mapping.Map(u => u.Name)
                .Not.Nullable()
                .Length(20);

            mapping.HasManyToMany(u => u.Users)
                .Table("UserUserGroup")
                .ParentKeyColumn("UserGroupId")
                .ChildKeyColumn("UserId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.SaveUpdate()
                .Inverse();
        }
    }
}