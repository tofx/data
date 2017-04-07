using System.Data;

namespace tofx.Data.Abstractions
{
    public class DbColumnDescriptor
    {
        public string ColumnName { get; set; }
        public string LegalColumnName { get; set; }
        public string Description { get; set; }
        public DbType ColumnDbType { get; set; }
        public int Length { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsComputed { get; set; }
        public bool IsPrimaryKey { get; set; }
    }
}