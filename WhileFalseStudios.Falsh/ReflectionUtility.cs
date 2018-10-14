using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WhileFalseStudios.Falsh
{
    public class ReflectionUtility
    {
        public static Dictionary<Type, T> GetTypesWithAttribute<T>(Assembly assembly) where T : Attribute
        {
            Dictionary<Type, T> ret = new Dictionary<Type, T>();

            var typesWithMyAttribute =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(T), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<T>() };

            foreach (var t in typesWithMyAttribute)
            {
                ret.Add(t.Type, t.Attributes.First());
            }

            return ret;
        }
    }
}
