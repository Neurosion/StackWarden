using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace StackWarden.UI.DependencyResolution
{
    public class ControllerActivator : IHttpControllerActivator
    {
        public ControllerActivator(HttpConfiguration configuration) { }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = IoC.Initialize().GetInstance(controllerType) as IHttpController;
            return controller;
        }
    }
}