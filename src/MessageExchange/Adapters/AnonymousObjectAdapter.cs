using DotLiquid;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageExchange.Adapters
{
    public class AnonymousObjectAdapter : IHashAdapter<object>
    {
        public Hash AsHash(object model) => Hash.FromAnonymousObject(model, true);
    }
}
