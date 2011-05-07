using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Northwind.Entities;
using Northwind.FluentNHibernate;
using Northwind.FluentNHibernate.Conventions;

namespace NHibernateWorkshop.SessionFactoryBuilders
{
    public class SqlServerFluentSessionFactoryBuilder : ISessionFactoryBuilder
    {
        public ISessionFactory BuildSessionFactory()
        {
            return Fluently.Configure()
                .Database(MsSqlConfiguration
                    .MsSql2008
                    .ConnectionString("Server=localhost;initial catalog=NHibernateWorkshop;Integrated Security=SSPI")
                    .ShowSql()
                    .FormatSql()
                    .AdoNetBatchSize(100))
                .Mappings(m => m.AutoMappings.Add(CreateAutomappings))
                .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(true, true))
                .BuildSessionFactory();
        }

        public static AutoPersistenceModel CreateAutomappings()
        {
            return AutoMap.AssemblyOf<Employee>(new AutomappingConfiguration())
                .Conventions.Setup(c => c.Add<CustomForeignKeyConvention>())
                .UseOverridesFromAssemblyOf<Employee>()
                .IncludeBase<ThirdParty>();
        }
    }
}