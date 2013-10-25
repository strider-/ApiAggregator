using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ApiAggregator.Core.Data;
using ApiAggregator.Core.Models;
using Ninject;

namespace ApiAggregator.Web.Filters
{
    public class EnsureSetupAttribute : ActionFilterAttribute
    {
        [Inject]
        public IConfigurationRepository ConfigurationRepository { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.RouteData.Values["controller"] as string;
            var action = filterContext.RouteData.Values["action"] as string;

            if(!(controller.Equals("Account", StringComparison.InvariantCultureIgnoreCase) && action.Equals("Setup", StringComparison.InvariantCultureIgnoreCase)))
            {
                var option = ConfigurationRepository.All().Single();
                if(string.IsNullOrWhiteSpace(option.Username) || string.IsNullOrWhiteSpace(option.Password))
                {
                    var result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Account",
                        action = "Setup"
                    }));
                    filterContext.Result = result;                
                }
                // The option is pulled on just about every request, lets share it
                // for the logoutbutton partial to avoid 2 DB hits for the same object
                filterContext.Controller.ViewBag.Option = option;
            }
                        
            base.OnActionExecuting(filterContext);
        }
    }
}