using System;
using System.Data;
using System.Data.SqlClient;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.ParameterNodes
{
    public class SqlDbParamCharNode : DbParameterNode
    {
        public SqlDbParamCharNode(IDbParameterParser parser) : base(parser)
        {
        }
        
        public override IDbDataParameter GetDbParameter()
        {
            if (MapProperty == null)
                throw new InvalidOperationException("MissingPropertyInfo");
            if (MapPropertyType == null)
                throw new InvalidOperationException("MissingPropertyType");

            if (MapPropertyType != typeof(char))
            {
                if (Next != null)
                    return Next.GetDbParameter();
            }

            var dbType = DbType.AnsiStringFixedLength;
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
