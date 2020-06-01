using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure.Ioc
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AutowriedAttribute : Attribute
    {

    }
}
