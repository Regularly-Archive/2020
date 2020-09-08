using MessageExchange.Extension;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageExchange.Templated
{
    public class JsonTransformer : IMessageTransformer
    {
        public string Template { get; private set; }

        public JObject Model { get; private set; }

        public string Transfrom()
        {
            var template = DotLiquid.Template.Parse(Template);
            return template.Render(DotLiquidExtension.FromJson(Model));
        }
    }
}
