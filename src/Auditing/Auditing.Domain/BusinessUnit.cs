using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Domain
{
    [Table("BusinessUnit")]
    public class BusinessUnit : BaseEntity<int>
    {
        public string OrgName { get; set; }
        public string OrgCode { get; set; }
        public string IsActive { get; set; }
        public string ParentOrg { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}
