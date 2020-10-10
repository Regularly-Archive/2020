using DotLiquid;
using MessageExchange.Extension;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageExchange.Adapters
{
    public class JsonAdapter : IHashAdapter<JObject>
    {
        public Hash AsHash(JObject model) => DotLiquidExtension.FromJson(model);
    }
}
