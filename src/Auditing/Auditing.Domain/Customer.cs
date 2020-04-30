using Dapper.Contrib.Extensions;
using System;

namespace Auditing.Domain
{
    [Table("Customer")]
    public class Customer : BaseEntity<int>
    {
        public string Tel { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
