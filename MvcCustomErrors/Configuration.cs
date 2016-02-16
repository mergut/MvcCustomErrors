using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcCustomErrors
{
    public static class Configuration
    {
        static Configuration()
        {
            ControllerName = "Error";
            ViewNamePrefix = "Http";
        }

        public static string ControllerName
        {
            get;
            set;
        }

        public static string ViewNamePrefix
        {
            get;
            set;
        }
    }
}
