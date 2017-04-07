﻿using System;
using System.Data;
using System.Data.SqlClient;
using TOF.Data.Abstractions;

namespace TOF.Data.Providers.SqlServer.ParameterNodes
{
    public class SqlDbParamDateTimeNode : DbParameterNode
    {
        public SqlDbParamDateTimeNode(IDbParameterParser parser) : base(parser) 
        {
        }
        
        public override IDbDataParameter GetDbParameter()
        {
            if (MapProperty == null)
                throw new InvalidOperationException("MissingPropertyInfo");
            if (MapPropertyType == null)
                throw new InvalidOperationException("MissingPropertyType");

            if (MapPropertyType != typeof(DateTime))
            {
                if (Next != null)
                    return Next.GetDbParameter();
            }

            var dbType = DbType.DateTime;
            var size = 8;

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
