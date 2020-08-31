using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using NUnit.Framework;
using System;
using System.ComponentModel.Design;
using System.Threading;

namespace ObjectPool.Test
{
    public class Tests
    {
        private IServiceCollection _service;
        private IServiceProvider _serviceProvider;
        [SetUp]
        public void Setup()
        {
            _service = new ServiceCollection();

            //使用DefaultObjectPoolProvider
            _service.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

            //使用默认策略
            _service.AddSingleton<ObjectPool<Foo>>(serviceProvider =>
            {
                var objectPoolProvider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                return objectPoolProvider.Create<Foo>();
            });

            //使用自定义策略
            _service.AddSingleton<ObjectPool<Foo>>(serviceProvider =>
            {
                var objectPoolProvider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                return objectPoolProvider.Create(new FooObjectPoolPolicy());
            });

            _serviceProvider = _service.BuildServiceProvider();
        }

        [Test]
        public void Test1()
        {
            var objectPool = _serviceProvider.GetService<ObjectPool<Foo>>();

            //有借有还，两次是同一个对象
            var item1 = objectPool.Get();
            objectPool.Return(item1);
            var item2 = objectPool.Get();
            Assert.AreEqual(item1, item2);//true

            //有借无还，两次是不同的对象
            var item3 = objectPool.Get();
            var item4 = objectPool.Get();
            Assert.AreEqual(item3, item4);//false
        }
    }

    public class Foo
    {
        public string Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }

    public class FooObjectPoolPolicy : IPooledObjectPolicy<Foo>
    {
        public Foo Create()
        {
            return new Foo()
            {
                Id = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.Now,
                CreatedBy = "Ezio"
            };
        }

        public bool Return(Foo obj)
        {
            return true;
        }
    }

}