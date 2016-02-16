using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcCustomErrors
{
    public class ErrorViewResult : ViewResult
    {
        public ErrorViewResult(IView view)
        {
            this.View = view;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.StatusCode = GetStatusCode(context);
            context.HttpContext.Response.TrySkipIisCustomErrors = true;

            base.ExecuteResult(context);
        }

        internal static int GetStatusCode(ControllerContext context)
        {
            object statusCodeObj;
            if (context.RouteData.DataTokens.TryGetValue("statusCode", out statusCodeObj))
            {
                return (int)statusCodeObj;
            }

            int statusCode;
            if (TryParseStatusCodeFromUrl(context.HttpContext.Request.Url, out statusCode))
            {
                return statusCode;
            }

            return 500;
        }

        internal static bool TryParseStatusCodeFromUrl(Uri url, out int statusCode)
        {
            statusCode = 0;
            string query = url.Query;
            if (query != null && query.Length >= 4)
            {
                return int.TryParse(query.Substring(1, 3), NumberStyles.Integer, CultureInfo.InvariantCulture, out statusCode);
            }

            return false;
        }
    }
}
