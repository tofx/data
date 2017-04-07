using tofx.Core.Abstractions;
using System;

namespace tofx.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MinLengthAttribute : Attribute, IValidateAttribute
    {
        public int Length { get; private set; }
        public MinLengthAttribute(int Length)
        {
            this.Length = Length;
        }

        public bool IsValid(object Value)
        {
            if (Value == null)
                throw new ArgumentNullException("E_VALIDATE_VALUE_IS_NULL");

            string val = Value.ToString();
            return val.Length >= this.Length;
        }
    }
}
