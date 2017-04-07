using TOF.Data.Abstractions;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace TOF.Data.Providers.SqlServer.InspectorStrategies
{
    public class SqlDbTableInspectorStrategy : IDbTableInspectorStrategy
    {
        public IEnumerable<IDbDataParameter> GetDbParameters()
        {
            return new List<IDbDataParameter>()
            {
                new SqlParameter("@tablename", SqlDbType.NVarChar, 250)
            };
        }

        public string GetDbQueryScript()
        {
            return @"SELECT t.name, ISNULL(ep2.value, '') AS TableDesc, c.name, 
                            dt.user_type_id, dt.name, c.max_length, c.is_nullable, 
                            c.is_identity, c.is_computed,
	                        CASE WHEN idx.key_ordinal IS NULL THEN 0 ELSE 1 END AS Idx,
	                        ISNULL((
	                            SELECT idx.is_primary_key 
                                FROM sys.index_columns idxc INNER JOIN sys.indexes idx ON idxc.index_id = idx.index_id
		                        WHERE idx.name = k.name AND idxc.object_id = t.object_id AND idxc.column_id = c.column_id
	                        ), 0) AS IsPrimaryKey,
                            ISNULL(ep.value, '') AS ColumnDesc
                    FROM sys.all_columns c 
                            INNER JOIN sys.tables t ON c.object_id = t.object_id
                            INNER JOIN sys.types dt ON c.user_type_id = dt.user_type_id
		                    LEFT JOIN sys.index_columns idx ON t.object_id = idx.object_id AND c.column_id = idx.column_id
		                    LEFT JOIN sys.key_constraints k ON c.object_id = k.parent_object_id
		                    LEFT JOIN sys.extended_properties ep ON c.object_id = ep.major_id AND c.column_id = ep.minor_id AND ep.name = 'MS_Description'
		                    LEFT JOIN sys.extended_properties ep2 ON c.object_id = ep2.major_id AND ep2.minor_id = 0 AND ep2.name = 'Schema_Description'
                    WHERE t.type = 'U' AND t.name = @tablename
                    ORDER BY t.name, c.column_id";
        }
    }
}
