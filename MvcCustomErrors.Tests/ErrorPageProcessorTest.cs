// Copyright (c) Mehmet Antoine Ergut
// Licensed under the MIT License (MIT). See LICENSE file in the project root for full license information.

namespace MvcCustomErrors.Tests
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using FakeSystemWeb;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ErrorPageProcessorTest
    {
        [Test]
        public void ErrorPageProcessor_GetStatusCode_WithNullExceptionParameter_Returns500()
        {
            this.GetStatusCodeTest(null, 500);
        }

        [Test]
        public void ErrorPageProcessor_GetStatusCode_WithNonHttpExceptionParameter_Returns500()
        {
            this.GetStatusCodeTest(new Exception(), 500);
        }

        [Test]
        public void ErrorPageProcessor_GetStatusCode_WithHttpExceptionParameter_ReturnsHttpCode()
        {
            this.GetStatusCodeTest(new HttpException(403, "Forbidden"), 403);
        }

        [Test]
        public void ErrorPageProcessor_CreateController_WhenFactoryThrows_Throws()
        {
            var processor = new ErrorPageProcessor();
            var exception = new HttpException(500, "Error");
            var mockControllerFactory = new Mock<IControllerFactory>();
            mockControllerFactory
                .Setup(m => m.CreateController(It.IsAny<RequestContext>(), It.IsAny<string>()))
                .Throws(exception);

            TestDelegate act = () =>
            {
                processor.CreateController(mockControllerFactory.Object, new RequestContext(), "no-controller");
            };

            Assert.That(act, Throws.Exception.EqualTo(exception));
        }

        [Test]
        public void ErrorPageProcessor_CreateController_WhenControllerNotFound_ReturnsNull()
        {
            var processor = new ErrorPageProcessor();
            var mockControllerFactory = new Mock<IControllerFactory>();
            mockControllerFactory
                .Setup(m => m.CreateController(It.IsAny<RequestContext>(), It.IsAny<string>()))
                .Throws(new HttpException(404, "Not Found"));

            var result = processor.CreateController(mockControllerFactory.Object, new RequestContext(), "no-controller");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ErrorPageProcessor_ProcessRequest_WhenControllerNotFound_Throws()
        {
            var processor = new ErrorPageProcessor();
            var mockControllerFactory = new Mock<IControllerFactory>();
            mockControllerFactory
                .Setup(m => m.CreateController(It.IsAny<RequestContext>(), It.IsAny<string>()))
                .Returns<IController>(null);

            TestDelegate act = () =>
            {
                processor.ProcessRequest(new FakeHttpContext(), mockControllerFactory.Object, "no-controller");
            };

            Assert.That(act, Throws.InvalidOperationException.With.Message.EqualTo("Cannot find a controller with name 'no-controller'."));
        }

        [Test]
        public void ErrorPageProcessor_ProcessRequest_WhenControllerFound_ExecutesController()
        {
            var processor = new ErrorPageProcessor();
            var mockController = new Mock<IController>();
            var controller = mockController.Object;
            var mockControllerFactory = new Mock<IControllerFactory>();
            mockControllerFactory
                .Setup(m => m.CreateController(It.IsAny<RequestContext>(), It.IsAny<string>()))
                .Returns(controller);

            processor.ProcessRequest(new FakeHttpContext(), mockControllerFactory.Object, "controller");

            mockController.Verify(m => m.Execute(It.IsAny<RequestContext>()), Times.Once);
            mockControllerFactory.Verify(m => m.ReleaseController(controller), Times.Once);
        }

        private void GetStatusCodeTest(Exception exception, int expected)
        {
            var processor = new ErrorPageProcessor();

            int result = processor.GetStatusCode(exception);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
