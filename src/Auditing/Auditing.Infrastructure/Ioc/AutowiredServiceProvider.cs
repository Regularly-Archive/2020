using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Auditing.Infrastructure.Ioc
{
    public class AutowiredServiceProvider : IServiceProvider,ISupportRequiredService
    {
        private readonly IServiceProvider _serviceProvider;
        public AutowiredServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetRequiredService(Type serviceType)
        {
            return GetService(serviceType);
        }

        public object GetService(Type serviceType)
        {
            var instance = _serviceProvider.GetService(serviceType);
            Autowried(instance);
            return instance;
        }

        private void Autowried(object instance)
        {
            if (_serviceProvider == null || instance == null)
                return;

            var flags = BindingFlags.Public | BindingFlags.NonPublic;
            var type = instance as Type ?? instance.GetType();
            if (instance is Type)
            {
                instance = null;
                flags |= BindingFlags.Static;
            }
            else
            {
                flags |= BindingFlags.Instance;
            }

            //Feild
            foreach (var field in type.GetFields(flags))
            {
                var autowriedAttr = field.GetCustomAttribute<AutowiredAttribute>();
                if (autowriedAttr != null)
                {
                    var dependency = GetService(field.FieldType);
                    if (dependency != null)
                        field.SetValue(instance, dependency);
                }
            }

            //Property
            foreach (var property in type.GetProperties(flags))
            {
                var autowriedAttr = property.GetCustomAttribute<AutowiredAttribute>();
                if (autowriedAttr != null)
                {
                    var dependency = GetService(property.PropertyType);
                    if (dependency != null)
                        property.SetValue(instance, dependency);
                }
            }
        }
    }
}
