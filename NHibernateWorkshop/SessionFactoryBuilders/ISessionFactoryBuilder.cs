using NHibernate;

namespace NHibernateWorkshop.SessionFactoryBuilders
{
    public interface ISessionFactoryBuilder
    {
        ISessionFactory BuildSessionFactory();
    }
}