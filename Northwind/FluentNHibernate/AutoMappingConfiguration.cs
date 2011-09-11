using FluentNHibernate.Automapping;

namespace Northwind.FluentNHibernate
{
    public class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(System.Type type)
        {
            // there's a bug in FluentNHibernate 1.3 where it automatically tries to map compiler generated classes
            return type.Namespace == "Northwind.Entities" && type.Name.IndexOf("DisplayClass") < 0;
        }

        public override bool IsComponent(System.Type type)
        {
            return type.Namespace == "Northwind.Components";
        }
    }
}