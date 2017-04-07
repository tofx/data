using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace tofx.Data.Annotations
{
    public static class TypeExtension
    {
        public static Boolean IsAnonymousType(this Type type)
        {
            Boolean hasCompilerGeneratedAttribute =
                type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
            Boolean nameContainsAnonymousType =
                type.FullName.Contains("AnonymousType");
            Boolean isAnonymousType =
                hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return isAnonymousType;
        }

        public static bool IsNullable(this Type type)
        {
            if (!type.IsValueType) return false; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }
    }
}
