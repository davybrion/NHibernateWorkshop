using System;
using log4net;
using NHibernate;
using NHibernateWorkshop.SessionFactoryBuilders;

namespace NHibernateWorkshop
{
    public abstract class Fixture
    {
        protected static ILog Logger { get; private set; }
        protected static ISessionFactory SessionFactory { get; private set; }

        static Fixture()
        {
            log4net.Config.XmlConfigurator.Configure();
            Logger = LogManager.GetLogger(typeof(Fixture));

            SetSessionFactory();
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                TestData.Create(session);
                transaction.Commit();
            }
        }

        private static void SetSessionFactory()
        {
#if FLUENTSQLITE
            SessionFactory = new SQLiteFluentSessionFactoryBuilder().BuildSessionFactory();
#elif FLUENTSQLSERVER
            SessionFactory = new SqlServerFluentSessionFactoryBuilder().BuildSessionFactory();
#elif HBMSQLSERVER
            SessionFactory = new SqlServerHbmSessionFactoryBuilder().BuildSessionFactory();
#else
            SessionFactory = new SQLiteHbmSessionFactoryBuilder().BuildSessionFactory();
#endif
        }

        protected static ISession CreateSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}