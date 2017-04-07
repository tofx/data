using TOF.Core.Utils.TypeExtensions;
using System;
using System.Data;
using System.Reflection;
using TOF.Core.Utils;
using TOF.Data.Abstractions;

namespace TOF.Data
{
    public abstract class DbParameterNode : IDbParameterChainNode
    {
        protected IDbParameterChainNode Next { get; set; }
        protected IDbParameterParser Parser { get; set; }
        protected PropertyInfo MapProperty { get; set; }
        protected Type MapPropertyType { get; set; }
        protected IDbModelPropertyStrategy DbPropertyStrategy { get; set; }

        protected DbParameterNode(IDbParameterParser parser)
        {
            ParameterChecker.NotNull(parser);

            Parser = parser;
            Initialize();
        }

        public abstract IDbDataParameter GetDbParameter();
        public IDbParameterChainNode NextNode(IDbParameterChainNode nodeNext)
        {
            Next = nodeNext;
            return this;
        }

        private void Initialize()
        {
            var propBindingInfo = Parser.GetModelPropertyStrategy();

            if (propBindingInfo == null && Parser.GetPropertyInfo() == null)
                throw new InvalidOperationException("UNABLE_DETERMINE_PARAMETER_TYPE");

            DbPropertyStrategy = propBindingInfo;
            MapProperty = (DbPropertyStrategy != null)
                ? DbPropertyStrategy.GetPropertyInfo()
                : Parser.GetPropertyInfo();

            var propertyType = MapProperty.PropertyType;

            if (propertyType != typeof(string) && 
                propertyType != typeof(DBNull) && 
                propertyType.IsNullable())
                propertyType = Nullable.GetUnderlyingType(propertyType);

            MapPropertyType = propertyType;
        }
    }
}
