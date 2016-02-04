// Copyright (c) 2016 Mehmet Antoine Ergut
// Licensed under the MIT License (MIT). See LICENSE in the project root for license information.

namespace MvcCustomErrors
{
    using System;
    using System.Web;
    using System.Web.UI;

    public class ErrorPage : Page
    {
        public override void ProcessRequest(HttpContext context)
        {
            int statusCode = 500;
            HttpException httpEx = context.Server.GetLastError() as HttpException;
            if (httpEx != null)
            {
                statusCode = httpEx.GetHttpCode();
            }

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.TrySkipIisCustomErrors = false;
            context.Response.SuppressContent = true;
        }
    }
}
