using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure.Services
{
    public class BarService : IBarService
    {
        public string Bar() => "I am Bar";
    }

    public interface IBarService
    {
        string Bar();
    }
}
