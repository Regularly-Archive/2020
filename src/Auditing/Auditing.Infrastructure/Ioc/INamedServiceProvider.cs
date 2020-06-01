using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure.Ioc
{ 
    public interface INamedServiceProvider
    {
        TService GetService<TService>(string serviceName);
    }
}
