// Copyright (c) Mehmet Antoine Ergut
// Licensed under the MIT License (MIT). See LICENSE file in the project root for full license information.

namespace MvcCustomErrors
{
    using System;

    /// <summary>
    /// Provides configuration options for MvcCustomErrors.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Intended to be used with the full name.")]
    public static class Configuration
    {
        /// <summary>
        /// Initializes static members of the <see cref="Configuration"/> class.
        /// </summary>
        static Configuration()
        {
            ControllerName = "Error";
            ViewNamePrefix = "Http";
        }

        /// <summary>
        /// Gets or sets the name of the error controller.
        /// </summary>
        public static string ControllerName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the view name prefix.
        /// </summary>
        public static string ViewNamePrefix
        {
            get;
            set;
        }
    }
}
