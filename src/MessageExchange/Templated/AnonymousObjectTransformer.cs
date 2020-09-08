using DotLiquid;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageExchange.Templated
{
    public class AnonymousObjectTransformer : IMessageTransformer
    {
        public string Template { get; private set; }

        public dynamic Model { get; private set; }

        public string Transfrom()
        {
            var template = DotLiquid.Template.Parse(Template);
            return template.Render(Hash.FromAnonymousObject(Model, true));
        }
    }
}
