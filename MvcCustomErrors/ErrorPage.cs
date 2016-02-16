// Copyright (c) Mehmet Antoine Ergut
// Licensed under the MIT License (MIT). See LICENSE file in the project root for full license information.

namespace MvcCustomErrors
{
    using System;
    using System.Globalization;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.UI;

    /// <summary>
    /// WebForm error page called from &gt;customErrors> section.
    /// </summary>
    /// <seealso cref="System.Web.UI.Page" />
    public class ErrorPage : Page
    {
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void ProcessRequest(HttpContext context)
        {
            HttpContextBase httpContext = new HttpContextWrapper(context);
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();

            this.ProcessRequest(httpContext, factory);
        }

        internal void ProcessRequest(HttpContextBase httpContext, IControllerFactory controllerFactory)
        {
            string controllerName = Configuration.ControllerName;
            RequestContext requestContext = this.CreateRequestContext(httpContext, controllerName);
            IController controller = this.CreateController(controllerFactory, requestContext, controllerName);
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
            int statusCode = this.GetStatusCode(httpContext.Server.GetLastError());

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
