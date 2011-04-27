using System.Reflection;

namespace NHibernateWorkshop
{
    public static class ReflectionHelper
    {
        public static object GetPrivateFieldValue(object theObject, string fieldName)
        {
            var type = theObject.GetType();
            return type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(theObject);
        }
    }
}