// Copyright (c) Mehmet Antoine Ergut
// Licensed under the MIT License (MIT). See LICENSE file in the project root for full license information.

namespace MvcCustomErrors
{
    using System;
    using System.Web;
    using System.Web.Mvc;
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
            string controllerName = Configuration.ControllerName;

            ErrorPageProcessor processor = new ErrorPageProcessor();
            processor.ProcessRequest(httpContext, factory, controllerName);
        }
    }
}
