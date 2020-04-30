using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Domain
{
    public class BaseEntity<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
