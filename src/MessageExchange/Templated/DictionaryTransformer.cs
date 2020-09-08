using DotLiquid;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageExchange.Templated
{
    public class DictionaryTransformer : IMessageTransformer
    {
        public string Template { get; private set; }

        public IDictionary<string, object> Model { get; private set; }

        public DictionaryTransformer(IDictionary<string, object> model, string template)
        {
            Model = model;
            Template = template;
        }

        public string Transfrom()
        {
            var template = DotLiquid.Template.Parse(Template);
            return template.Render(Hash.FromDictionary(Model));
        }
    }
}
