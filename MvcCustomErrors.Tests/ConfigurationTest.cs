// Copyright (c) Mehmet Antoine Ergut
// Licensed under the MIT License (MIT). See LICENSE file in the project root for full license information.

namespace MvcCustomErrors.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class ConfigurationTest
    {
        [Test]
        public void Configuration_ControllerName_Set_SetsValue()
        {
            string random = Guid.NewGuid().ToString("N");
            Configuration.ControllerName = random;

            Assert.That(Configuration.ControllerName, Is.EqualTo(random));
        }

        [Test]
        public void Configuration_ViewNamePrefix_Set_SetsValue()
        {
            string random = Guid.NewGuid().ToString("N");
            Configuration.ViewNamePrefix = random;

            Assert.That(Configuration.ViewNamePrefix, Is.EqualTo(random));
        }
    }
}
