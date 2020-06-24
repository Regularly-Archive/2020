using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Auditing.Infrastructure.Ioc
{
    public class AutowiredControllerActivator : IControllerActivator
    {
        public object Create(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(ControllerContext));

            var controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();
            var serviceProvider = new AutowiredServiceProvider(context.HttpContext.RequestServices);
            var controller = serviceProvider.GetRequiredService(controllerType);
            return controller;
        }

        public void Release(ControllerContext context, object controller)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(ControllerContext));
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));

            var disposeable = controller as IDisposable;
            if (disposeable != null)
                disposeable.Dispose();
        }

        private void Autowried(IServiceProvider serviceProvider, object instance)
        {
            if (serviceProvider == null || instance == null)
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
                    var dependency = serviceProvider.GetRequiredService(field.FieldType);
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
                    var dependency = serviceProvider.GetRequiredService(property.PropertyType);
                    if (dependency != null)
                        property.SetValue(instance, dependency);
                }
            }
        }
    }
}
