using System;
using System.Web.Mvc;
using MvcCustomErrors;

namespace $rootnamespace$.Controllers
{
    public class ErrorController : Controller
    {
        protected override void HandleUnknownAction(string actionName)
        {
            ErrorControllerHelper.HandleUnknownAction(this.ControllerContext, actionName);
        }
    }
}
