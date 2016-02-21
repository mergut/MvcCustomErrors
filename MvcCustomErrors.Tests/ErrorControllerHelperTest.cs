// Copyright (c) Mehmet Antoine Ergut
// Licensed under the MIT License (MIT). See LICENSE file in the project root for full license information.

namespace MvcCustomErrors.Tests
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using FakeSystemWeb;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ErrorControllerHelperTest
    {
        [Test]
        public void ErrorControllerHelper_HandleUnknownAction_WithNullControllerContextParameter_Throws()
        {
            TestDelegate act = () =>
            {
                ErrorControllerHelper.HandleUnknownAction(null, "unused");
            };

            Assert.That(act, Throws.ArgumentNullException.With.Property("ParamName").EqualTo("controllerContext"));
        }

        [Test]
        public void ErrorControllerHelper_HandleUnknownAction_WithNullOrEmptyActionNameParameter_Throws()
        {
            var controllerContext = CreateControllerContext();

            TestDelegate act = () =>
            {
                ErrorControllerHelper.HandleUnknownAction(controllerContext, null);
            };

            Assert.That(act, Throws.ArgumentNullException.With.Property("ParamName").EqualTo("actionName"));
        }

        [Test]
        public void ErrorControllerHelper_HandleUnknownAction_WithNullOrEmptyDefaultViewNameParameter_Throws()
        {
            var controllerContext = CreateControllerContext();

            TestDelegate act = () =>
            {
                ErrorControllerHelper.HandleUnknownAction(controllerContext, "action", null);
            };

            Assert.That(act, Throws.ArgumentNullException.With.Property("ParamName").EqualTo("defaultViewName"));
        }

        [Test]
        public void ErrorControllerHelper_HandleUnknownAction_WithNoViews_SearchesForViewTwiceThenDisplaysError()
        {
            var viewEngineMock = new Mock<IViewEngine>();
            viewEngineMock
                .Setup(m => m.FindView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new ViewEngineResult(Enumerable.Empty<string>()));
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(viewEngineMock.Object);

            using (var httpOut = new FakeHttpOutput())
            {
                var httpContext = new FakeHttpContext(new FakeHttpRequest(), new FakeHttpResponse(httpOut));
                var controllerContext = CreateControllerContext(httpContext);

                ErrorControllerHelper.HandleUnknownAction(controllerContext, "action");

                viewEngineMock
                    .Verify(m => m.FindView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<string>(), true), Times.Exactly(2));
                viewEngineMock
                    .Verify(m => m.FindView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<string>(), false), Times.Exactly(2));

                Assert.That(httpContext.Response.ContentType, Is.EqualTo("text/html"));
                Assert.That(httpOut.GetContentString(), Does.StartWith("<!DOCTYPE html>"));
            }
        }

        [Test]
        public void ErrorControllerHelper_HandleUnknownAction_WithFoundView_ExecutesResult()
        {
            var viewMock = new Mock<IView>();

            var viewEngineMock = new Mock<IViewEngine>();
            viewEngineMock
                .Setup(m => m.FindView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new ViewEngineResult(viewMock.Object, viewEngineMock.Object));
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(viewEngineMock.Object);

            var httpContext = new FakeHttpContext();
            var routeData = new RouteData();
            routeData.Values["action"] = "action";
            var controllerContext = CreateControllerContext(httpContext, routeData);

            ErrorControllerHelper.HandleUnknownAction(controllerContext, "action");

            viewEngineMock.Verify(m => m.FindView(It.IsAny<ControllerContext>(), "action", It.IsAny<string>(), true), Times.Once);
            viewMock.Verify(m => m.Render(It.IsAny<ViewContext>(), httpContext.Response.Output), Times.Once);

            Assert.That(httpContext.Response.StatusCode, Is.EqualTo(500));
            Assert.That(httpContext.Response.TrySkipIisCustomErrors, Is.True);
        }

        private static ControllerContext CreateControllerContext(HttpContextBase httpContext = null, RouteData routeData = null)
        {
            var controllerMock = new Mock<ControllerBase>();

            return new ControllerContext(
                httpContext ?? new FakeHttpContext(),
                routeData ?? new RouteData(),
                controllerMock.Object);
        }
    }
}
