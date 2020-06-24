using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure.Services
{
    public interface ISayHello
    {
        string SayHello(string receiver);
    }

    public class ChineseSayHello : ISayHello
    {
        public string SayHello(string receiver)
        {
            return $"你好，{receiver}";
        }
    }

    public class EnglishSayHello : ISayHello
    {
        public string SayHello(string receiver)
        {
            return $"Hello，{receiver}";
        }
    }
}
