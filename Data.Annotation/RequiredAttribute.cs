using TOF.Core.Abstractions;
using System;

namespace TOF.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RequiredAttribute : Attribute, IValidateAttribute
    {
        public bool IsValid(object Value)
        {
            if (Value.GetType().IsValueType)
            {
                if (Value.GetType().IsNullable())
                {
                    object hasValue = Value.GetType().GetProperty("HasValue").GetValue(Value, null);

                    if (hasValue == null)
                        return false;
                    else
                        return Convert.ToBoolean(hasValue);
                }
                else
                    return true;
            }
            else
                return Value != null;
        }
    }
}
