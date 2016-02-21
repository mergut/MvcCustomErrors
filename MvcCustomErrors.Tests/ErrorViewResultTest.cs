// Copyright (c) Mehmet Antoine Ergut
// Licensed under the MIT License (MIT). See LICENSE file in the project root for full license information.

namespace MvcCustomErrors.Tests
{
    using System;
    using System.Web.Mvc;
    using FakeSystemWeb;
    using NUnit.Framework;

    [TestFixture]
    public class ErrorViewResultTest
    {
        [Test]
        [TestCase("http://localhost/test")]
        [TestCase("http://localhost/test?")]
        [TestCase("http://localhost/test?12")]
        [TestCase("http://localhost/test?not-numeric")]
        public void ErrorViewResult_TryParseStatusCodeFromUrl_Failure(string url)
        {
            int statusCode;

            bool result = ErrorViewResult.TryParseStatusCodeFromUrl(new Uri(url), out statusCode);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("http://localhost/test?403", 403)]
        [TestCase("http://localhost/test?404;not-found", 404)]
        public void ErrorViewResult_TryParseStatusCodeFromUrl_Success(string url, int code)
        {
            int statusCode;

            bool result = ErrorViewResult.TryParseStatusCodeFromUrl(new Uri(url), out statusCode);

            Assert.That(result, Is.True);
            Assert.That(statusCode, Is.EqualTo(code));
        }

        [Test]
        public void ErrorViewResult_GetStatusCode_WithStatusCodeInRouteData_ReturnsCode()
        {
            var controllerContext = new ControllerContext();
            controllerContext.RouteData.DataTokens["statusCode"] = 403;

            int result = ErrorViewResult.GetStatusCode(controllerContext);

            Assert.That(result, Is.EqualTo(403));
        }

        [Test]
        public void ErrorViewResult_GetStatusCode_WithStatusCodeInUrl_ReturnsCode()
        {
            var controllerContext = new ControllerContext();
            controllerContext.HttpContext = new FakeHttpContext(new Uri("http://localhost/test?405"));

            int result = ErrorViewResult.GetStatusCode(controllerContext);

            Assert.That(result, Is.EqualTo(405));
        }

        [Test]
        public void ErrorViewResult_ExecuteResult_WithNullContextParameter_Throws()
        {
            var viewresult = new ErrorViewResult(null);

            TestDelegate act = () =>
            {
                viewresult.ExecuteResult(null);
            };

            Assert.That(act, Throws.ArgumentNullException.With.Property("ParamName").EqualTo("context"));
        }
    }
}
