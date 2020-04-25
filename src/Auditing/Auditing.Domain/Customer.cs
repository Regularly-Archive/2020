using System;

namespace Auditing.Domain
{
    public class Customer : BaseEntity<int>
    {
        public string Tel { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
