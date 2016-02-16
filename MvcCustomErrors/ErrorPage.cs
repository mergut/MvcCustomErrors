// Copyright (c) 2016 Mehmet Antoine Ergut
// Licensed under the MIT License (MIT). See LICENSE in the project root for license information.

namespace MvcCustomErrors
{
    using System;
    using System.Globalization;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.UI;

    public class ErrorPage : Page
    {
        public override void ProcessRequest(HttpContext context)
        {
            HttpContextBase httpContext = new HttpContextWrapper(context);
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();

            ProcessRequest(httpContext, factory);
        }

        internal void ProcessRequest(HttpContextBase httpContext, IControllerFactory controllerFactory)
        {
            string controllerName = Configuration.ControllerName;
            RequestContext requestContext = CreateRequestContext(httpContext, controllerName);
            IController controller = CreateController(controllerFactory, requestContext, controllerName);
            if (controller == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot find a controller with name '{0}'.", controllerName));
            }

            try
            {
                controller.Execute(requestContext);
            }
            finally
            {
                controllerFactory.ReleaseController(controller);
            }
        }

        internal IController CreateController(IControllerFactory controllerFactory, RequestContext requestContext, string controllerName)
        {
            try
            {
                return controllerFactory.CreateController(requestContext, controllerName);
            }
            catch (HttpException ex)
            {
                if (ex.GetHttpCode() == 404)
                {
                    return null;
                }

                throw;
            }
        }

        internal RequestContext CreateRequestContext(HttpContextBase httpContext, string controllerName)
        {
            int statusCode = GetStatusCode(httpContext.Server.GetLastError());

            RouteData routeData = new RouteData();
            routeData.DataTokens["statusCode"] = statusCode;
            routeData.Values["controller"] = controllerName;
            routeData.Values["action"] = Configuration.ViewNamePrefix + statusCode.ToString(CultureInfo.InvariantCulture);

            return new RequestContext(httpContext, routeData);
        }

        internal int GetStatusCode(Exception exception)
        {
            HttpException httpEx = exception as HttpException;
            if (httpEx != null)
            {
                return httpEx.GetHttpCode();
            }

            return 500;
        }
    }
}
