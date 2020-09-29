using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StormFog.Api.Rest.Models
{
    public class NodeConfig
    {
        public string HostName { get; set; }
        public List<NodeJob> Jobs { get; set; }
    }
}
