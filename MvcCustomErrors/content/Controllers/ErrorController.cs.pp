using System;
using System.Web.Mvc;

namespace $rootnamespace$.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult InternalServerError()
        {
            this.Response.StatusCode = 500;

            return View();
        }

        public ActionResult NotFound()
        {
            this.Response.StatusCode = 404;

            return View();
        }
    }
}
