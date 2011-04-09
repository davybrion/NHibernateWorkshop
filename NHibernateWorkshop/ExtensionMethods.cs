using System;
using System.Collections.Generic;

namespace NHibernateWorkshop
{
    public static class DateTimeExtension
    {
        // NOTE: the RemoveMillies extension method is not necessary when using HBM files. If you specify a 'timestamp'
        // type in your HBM files, the DateTime instances will contain the correct milliseconds. I couldn't get 
        // FluentNHibernate to produce the same behavior as a property mapped as timestamp with HBM, so i'm falling back
        // to this method to keep the tests passing in both cases

        public static DateTime RemoveMillies(this DateTime source)
        {
            return new DateTime(source.Ticks - (source.Ticks % TimeSpan.TicksPerSecond), source.Kind);
        }

        public static DateTime? RemoveMillies(this DateTime? source)
        {
            if (source == null) return null;
            return source.Value.RemoveMillies();
        }
    }

    public static class EnumerableExtensions
    {
        public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }
    }
}