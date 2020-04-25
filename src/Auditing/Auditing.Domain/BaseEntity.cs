using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Domain
{
    public class BaseEntity<T>
    {
        public T Id { get; set; }
    }
}
