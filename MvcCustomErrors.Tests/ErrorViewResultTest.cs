// Copyright (c) Mehmet Antoine Ergut
// Licensed under the MIT License (MIT). See LICENSE file in the project root for full license information.

namespace MvcCustomErrors.Tests
{
    using System;
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
    }
}
