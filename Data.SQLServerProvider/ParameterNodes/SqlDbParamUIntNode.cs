﻿using System;
using System.Data;
using System.Data.SqlClient;
using tofx.Data.Abstractions;

namespace tofx.Data.Providers.SqlServer.ParameterNodes
{
    public class SqlDbParamUIntNode : DbParameterNode
    {
        public SqlDbParamUIntNode(IDbParameterParser parser) : base(parser) 
        {
        }

        public override IDbDataParameter GetDbParameter()
        {
            if (MapProperty == null)
                throw new InvalidOperationException("MissingPropertyInfo");
            if (MapPropertyType == null)
                throw new InvalidOperationException("MissingPropertyType");

            if (MapPropertyType != typeof(uint))
            {
                if (Next != null)
                    return Next.GetDbParameter();
            }

            var dbType = DbType.Int32; // SQL Server Implementation.
            var size = 4;

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
