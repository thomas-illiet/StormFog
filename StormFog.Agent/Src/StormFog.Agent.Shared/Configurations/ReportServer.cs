using System;
using System.Collections.Generic;
using System.Text;

namespace StormFog.Agent.Shared.Configurations
{
    public class ReportServer
    {
        public string ServerURL { get; set; }
        public Guid RegistrationKey { get; set; }
        public string ProxyURL { get; set; }
    }
}
