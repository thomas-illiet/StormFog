using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StormFog.Api.Rest.Models
{
    public class NodeJob
    {
        public string Name { get; set; }
        public string ScheduleExpression { get; set; }
        public bool Status { get; set; }
    }
}
