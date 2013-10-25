using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using ApiAggregator.Web.Filters;
using ApiAggregator.Web.ViewModels;

namespace ApiAggregator.Web.Controllers
{
    [EnsureSetup]
    public class BaseController<T> : Controller where T : IController
    {
        private Dictionary<FlashType, FlashModel> _messages;

        public BaseController()
        {
            _messages = new Dictionary<FlashType, FlashModel>();
        }

        protected RedirectToRouteResult RedirectToAction(Expression<Func<T, object>> action)
        {
            return RedirectToAction<T>(action);
        }

        protected RedirectToRouteResult RedirectToAction<C>(Expression<Func<C, object>> action) where C : IController
        {
            if(!(action.Body is MethodCallExpression))
            {
                throw new ArgumentException("Redirect action must be a method.", "action");
            }

            var result = new ExpressionRedirectResult<C>(action);

            return RedirectToAction(result.Action, result.Controller, result.RouteValues);
        }

        protected JsonResult Json(bool success, string message)
        {
            return Json(new { Success = success, Message = message }, "application/json", JsonRequestBehavior.DenyGet);
        }

        protected void Flash(FlashType type, string message)
        {
            Flash(type, null, message);
        }

        protected void Flash(FlashType type, string title, string message)
        {
            _messages[type] = new FlashModel { Emphasis = title, Message = message };
            TempData["Flash"] = _messages;
        }

        private class ExpressionRedirectResult<TController> where TController : IController
        {
            public ExpressionRedirectResult(Expression<Func<TController, object>> exp)
            {
                var cType = typeof(TController);
                Controller = cType.Name.Substring(0, cType.Name.LastIndexOf("Controller"));

                var method = (exp.Body as MethodCallExpression);
                Action = method.Method.Name;

                var dict = new Dictionary<string, object>();
                var parms = method.Method.GetParameters();
                for(int i = 0; i < method.Arguments.Count; i++)
                {
                    var name = parms[i].Name;
                    var unary = Expression.Convert(method.Arguments[i], typeof(object));
                    var getValue = Expression.Lambda<Func<object>>(unary).Compile();
                    dict[name] = getValue();
                }
                RouteValues = new RouteValueDictionary(dict);
            }

            public string Action { get; set; }
            public string Controller { get; set; }
            public RouteValueDictionary RouteValues { get; set; }
        }
    }
}