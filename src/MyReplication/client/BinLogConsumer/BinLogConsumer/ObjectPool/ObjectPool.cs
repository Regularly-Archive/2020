using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace BinLogConsumer.ObjectPool
{
    public class ObjectPool<T> : IObjectPool<T>
    {
        private Func<T> _instanceFactory;
        private ConcurrentBag<T> _instanceItems;

        public ObjectPool(Func<T> instanceFactory)
        {
            _instanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
            _instanceItems = new ConcurrentBag<T>();
        }
        public T Get()
        {
            T item;
            if (_instanceItems.TryTake(out item)) return item;
            return _instanceFactory();
        }

        public void Return(T item)
        {
            _instanceItems.Add(item);
        }
    }
}
