using System;
using System.Data;
using System.Data.SqlClient;
using tofx.Core.Utils.TypeExtensions;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.ParameterNodes
{
    public class SqlDbParamBooleanNode : DbParameterNode
    {
        public SqlDbParamBooleanNode(IDbParameterParser parser) : base(parser)
        {
        }
        
        public override IDbDataParameter GetDbParameter()
        {
            if (MapProperty == null)
                throw new InvalidOperationException("MissingPropertyInfo");
            if (MapPropertyType == null)
                throw new InvalidOperationException("MissingPropertyType");

            if (MapPropertyType != typeof(bool))
            {
                if (Next != null)
                    return Next.GetDbParameter();
            }

            var dbType = DbType.Boolean;
            var size = 1;

            if (DbPropertyStrategy != null)
            {
                var dbTypeValue = DbPropertyStrategy.GetMapDbType();
                var sizeValue = DbPropertyStrategy.GetLength();

                if (dbTypeValue.HasValue)
                    dbType = dbTypeValue.Value;
                if (sizeValue.HasValue)
                    size = sizeValue.Value;
            }

            var param = new SqlParameter("@" + MapProperty.Name, null)
            {
                DbType = dbType,
                Size = size
            };

            return param;
        }
    }
}
