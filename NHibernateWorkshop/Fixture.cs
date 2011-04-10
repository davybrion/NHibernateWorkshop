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

#if FLUENTSQLITE
            SessionFactory = new SQLiteFluentSessionFactoryBuilder().BuildSessionFactory();
#else
            SessionFactory = new SQLiteHbmSessionFactoryBuilder().BuildSessionFactory();
#endif

            // TODO: remove this
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                TestData.Create(session);
                transaction.Commit();
            }
        }

        protected static ISession CreateSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}