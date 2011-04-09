using FluentNHibernate.Automapping;
using FluentNHibernate.Utils;
using Northwind.Entities;

namespace Northwind.FluentNHibernate
{
    public class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(System.Type type)
        {
            return type.Namespace == "Northwind.Entities";
        }

        public override bool IsComponent(System.Type type)
        {
            return type.Namespace == "Northwind.Components";
        }
    }
}