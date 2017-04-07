using System.Collections.Generic;

namespace TOF.Data.Abstractions
{
    public class DbTableDescriptor
    {
        public string TableName { get; set; }
        public string LegalEntityName { get; set; }
        public List<DbColumnDescriptor> Columns { get; set; }
        public string Description { get; set; }
    }
}
