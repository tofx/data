using System;

namespace tofx.Data
{
    public static class ClrTypeHelper
    {
        public static string GetFriendlyNameFromClrType(Type clrType, bool isNullable)
        {
            string clrTypeName = clrType.Name;
            bool isArray = clrType.IsArray;

            if (isArray)
                clrTypeName = clrTypeName.Replace("[]", "");

            switch (clrTypeName)
            {
                case "Int32":
                    return (isArray) ? "int[]" : "int" + ((isNullable) ? "?" : "");
                case "Int16":
                    return (isArray) ? "short[]" : "short" + ((isNullable) ? "?" : "");
                case "Int64":
                    return (isArray) ? "long[]" : "long" + ((isNullable) ? "?" : "");
                case "Single":
                    return (isArray) ? "float[]" : "float" + ((isNullable) ? "?" : "");
                case "Double":
                    return (isArray) ? "double[]" : "double" + ((isNullable) ? "?" : "");
                case "Boolean":
                    return (isArray) ? "bool[]" : "bool" + ((isNullable) ? "?" : "");
                case "Decimal":
                    return (isArray) ? "decimal[]" : "decimal" + ((isNullable) ? "?" : "");
                case "String":
                    return (isArray) ? "string[]" : "string";
                case "Byte":
                    return (isArray) ? "byte[]" : "byte" + ((isNullable) ? "?" : "");
                case "Char":
                    return (isArray) ? "char[]" : "char" + ((isNullable) ? "?" : "");
                case "DateTime":
                    return (isArray) ? "DateTime[]" : "DateTime" + ((isNullable) ? "?" : "");
                default:
                    return clrType.Name;
            }
        }
    }
}
