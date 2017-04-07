using System;
using System.Data;
using System.Data.SqlClient;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.ParameterNodes
{
    public class SqlDbParamBytesNode : DbParameterNode
    {
        public SqlDbParamBytesNode(IDbParameterParser parser) : base(parser)
        {
        }
        
        public override IDbDataParameter GetDbParameter()
        {
            if (MapProperty == null)
                throw new InvalidOperationException("MissingPropertyInfo");
            if (MapPropertyType == null)
                throw new InvalidOperationException("MissingPropertyType");

            if (MapPropertyType != typeof(byte[]))
            {
                if (Next != null)
                    return Next.GetDbParameter();
            }

            var dbType = DbType.Binary;
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
