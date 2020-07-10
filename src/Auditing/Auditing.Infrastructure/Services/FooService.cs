using Auditing.Infrastructure.Ioc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure.Services
{
    public class FooService : IFooService
    {
        [Autowired]
        public IBarService Bar { get; set; }

        public string Foo() => "I am Foo";
    }

    public interface IFooService
    {
        string Foo();
        IBarService Bar { get; set; }
    }
}
