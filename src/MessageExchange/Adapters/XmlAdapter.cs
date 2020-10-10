using DotLiquid;
using DotLiquid.Exceptions;
using MessageExchange.Extension;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MessageExchange.Adapters
{
    public class XmlAdapter : IHashAdapter<XmlDocument>
    {
        public Hash AsHash(XmlDocument model) => DotLiquidExtension.FromXml(model);
    }
}
