using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Domain
{
    [Table("AuditLog")]
    public class AuditLog : BaseEntity<string>
    {
        public string TableName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string ExtraData { get; set; }
        public int OperationType { get; set; }
    }

    [Flags]
    public enum OperationType
    {
        Created = 10,
        Updated = 20,
        Deleted = 30,
    }
}
