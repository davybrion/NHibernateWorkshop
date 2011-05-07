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
    public class SQLiteFluentSessionFactoryBuilder : ISessionFactoryBuilder
    {
        public ISessionFactory BuildSessionFactory()
        {
            FileHelper.DeletePreviousDbFiles();
            var dbFile = FileHelper.GetDbFileName();

            return Fluently.Configure()
                .Database(SQLiteConfiguration
                    .Standard.UsingFile(dbFile)
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