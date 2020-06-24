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
            var serviceProvider = context.HttpContext.RequestServices;
            if(!(serviceProvider is AutowiredServiceProvider))
                serviceProvider = new AutowiredServiceProvider(context.HttpContext.RequestServices);
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
    }
}
