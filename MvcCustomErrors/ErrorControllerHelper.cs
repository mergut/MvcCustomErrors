// Copyright (c) Mehmet Antoine Ergut
// Licensed under the MIT License (MIT). See LICENSE file in the project root for full license information.

namespace MvcCustomErrors
{
    using System;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Provides helper methods for the error controller.
    /// </summary>
    public static class ErrorControllerHelper
    {
        /// <summary>
        /// Displays the error view.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionName">Name of the action.</param>
        public static void HandleUnknownAction(ControllerContext controllerContext, string actionName)
        {
            HandleUnknownAction(controllerContext, actionName, "Default");
        }

        /// <summary>
        /// Displays the error view.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="defaultViewName">Default name of the view.</param>
        public static void HandleUnknownAction(ControllerContext controllerContext, string actionName, string defaultViewName)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException(nameof(controllerContext));
            }

            if (string.IsNullOrEmpty(actionName))
            {
                throw new ArgumentNullException(nameof(actionName));
            }

            if (string.IsNullOrEmpty(defaultViewName))
            {
                throw new ArgumentNullException(nameof(defaultViewName));
            }

            if (!DisplayView(controllerContext, actionName, defaultViewName))
            {
                DisplayMissingViewError(controllerContext.HttpContext, actionName, defaultViewName);
            }
        }

        internal static bool DisplayView(ControllerContext controllerContext, params string[] candidateViewNames)
        {
            foreach (string viewName in candidateViewNames)
            {
                ViewEngineResult engineResult = ViewEngines.Engines.FindView(controllerContext, viewName, null);
                if (engineResult.View != null)
                {
                    ErrorViewResult viewResult = new ErrorViewResult(engineResult.View);
                    try
                    {
                        viewResult.ExecuteResult(controllerContext);
                    }
                    finally
                    {
                        engineResult.ViewEngine.ReleaseView(controllerContext, engineResult.View);
                    }

                    return true;
                }
            }

            return false;
        }

        internal static void DisplayMissingViewError(HttpContextBase httpContext, params string[] candidateViewNames)
        {
            HttpResponseBase httpResponse = httpContext.Response;
            httpResponse.ContentType = "text/html";
            httpResponse.ContentEncoding = Encoding.UTF8;

            TextWriter output = httpResponse.Output;
            output.WriteLine("<!DOCTYPE html>");
            output.WriteLine("<html>");
            output.WriteLine("    <head>");
            output.WriteLine("        <title>Runtime Error</title>");
            output.WriteLine("    </head>");
            output.WriteLine("    <body>");
            output.WriteLine("        <h1>Server Error</h1>");
            output.WriteLine("        <p>An error occurred while processing your request.</p>");
            if (httpContext.Request.IsLocal)
            {
                output.WriteLine("        <p><i>Additionally a view cannot be found when trying to render the error page: {0}.</i></p>", httpContext.Server.HtmlEncode(string.Join(", ", candidateViewNames)));
            }

            output.WriteLine("    </body>");
            output.WriteLine("</html>");

            httpResponse.Flush();
        }
    }
}
