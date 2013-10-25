using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace ApiAggregator.Web.Framework
{
    public class NotFoundControllerFactory : DefaultControllerFactory
    {
        private string _controller;
        private string _action;

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            if(string.IsNullOrWhiteSpace(_controller) && string.IsNullOrWhiteSpace(_action))
            {
                return base.CreateController(requestContext, controllerName);
            }

            if(requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }

            if(string.IsNullOrWhiteSpace(controllerName))
            {
                throw new ArgumentException("controllerName cannot be null or empty.");
            }

            var controllerType = GetControllerType(requestContext, controllerName);
            var actionName = requestContext.RouteData.Values["action"] as string;

            if(controllerType == null || !controllerType.GetMethods().Where(m => m.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase)).Any())
            {
                controllerName = _controller;
                controllerType = GetControllerType(requestContext, controllerName);
                requestContext.RouteData.Values["controller"] = controllerName;
                requestContext.RouteData.Values["action"] = _action;
            }

            return GetControllerInstance(requestContext, controllerType);
        }

        public void NotFoundAction<T>(Expression<Func<T, object>> selector) where T : IController
        {
            if(!(selector.Body is MethodCallExpression))
            {
                throw new ArgumentException("Not found action must be a method.", "selector");
            }

            var typeName = selector.Parameters[0].Type.Name;
            _controller = typeName.Substring(0, typeName.LastIndexOf("Controller"));
            _action = ((MethodCallExpression)selector.Body).Method.Name;
        }
    }
}