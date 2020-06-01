using Auditing.Infrastructure.Ioc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure.Services
{
    public class FooService : IFooService
    {
        [Autowried]
        public IBarService Bar { get; set; }

        public string Foo()
        {
            return "I am foo";
        }
    }

    public interface IFooService
    {
        string Foo();
    }
}
