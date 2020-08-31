using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinLogConsumer.ObjectPool
{
    public interface IObjectPool<T>
    {
        T Get();
        void Return(T obj);
    }
}
