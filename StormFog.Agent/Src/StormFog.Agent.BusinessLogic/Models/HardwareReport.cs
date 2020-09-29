using StormFog.Agent.BusinessLogic.Wmi.Hardware;
using System;
using System.Collections.Generic;
using System.Text;

namespace StormFog.Agent.BusinessLogic.Models
{
    public class HardwareReport
    {
        public ComputerSystem ComputerSystem { get; set; }
        public BootConfiguration BootConfiguration { get; set; }
    }
}
