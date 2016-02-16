using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcCustomErrors
{
    public static class ErrorControllerHelper
    {
        public static void HandleUnknownAction(ControllerContext controllerContext, string actionName, string defaultViewName = "Default")
        {
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
            httpResponse.Close();
        }
    }
}
