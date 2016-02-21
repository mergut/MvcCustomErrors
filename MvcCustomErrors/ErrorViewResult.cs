// Copyright (c) Mehmet Antoine Ergut
// Licensed under the MIT License (MIT). See LICENSE file in the project root for full license information.

namespace MvcCustomErrors
{
    using System;
    using System.Globalization;
    using System.Web.Mvc;

    /// <summary>
    /// Represents an error view result.
    /// </summary>
    /// <seealso cref="System.Web.Mvc.ViewResult" />
    public class ErrorViewResult : ViewResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorViewResult"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        public ErrorViewResult(IView view)
        {
            this.View = view;
        }

        /// <summary>
        /// Executes the result.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

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
            if (query.Length >= 4)
            {
                return int.TryParse(query.Substring(1, 3), NumberStyles.Integer, CultureInfo.InvariantCulture, out statusCode);
            }

            return false;
        }
    }
}
