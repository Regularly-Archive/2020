using DotLiquid;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageExchange.Adapters
{
    public class DictionaryAdapter : IHashAdapter<IDictionary<string, object>>
    {
        public Hash AsHash(IDictionary<string, object> model) => Hash.FromDictionary(model);
    }
}
