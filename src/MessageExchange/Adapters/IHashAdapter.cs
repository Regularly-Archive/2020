using DotLiquid;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageExchange.Adapters
{
    public interface IHashAdapter<TModel>
    {
        Hash AsHash(TModel model);
    }
}
