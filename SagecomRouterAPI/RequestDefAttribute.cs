using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagecomRouterAPI
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class RequestDefAttribute : Attribute
    {
        public string ConfigFile { get; set; }

        public RequestDefAttribute(string configFile)
        {
            ConfigFile = configFile;
        }
    }
}
