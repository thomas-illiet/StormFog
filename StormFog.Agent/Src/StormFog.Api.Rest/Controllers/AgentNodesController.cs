using Microsoft.AspNetCore.Mvc;
using StormFog.Api.Rest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StormFog.Api.Rest.Controllers
{
    [ApiController]
    [Route("agent/nodes")]
    public class AgentNodesController
    {
        public AgentNodesController()
        {

        }

        [HttpGet("{nodeId:guid}/config")]
        public NodeConfig GetNodeConfig(Guid nodeId)
        {
            var config = new NodeConfig();
            config.HostName = "LAPTOP-EBJVS28";

            var jobs = new List<NodeJob>();
            jobs.Add(new NodeJob()
            {
                Name = "Test",
                ScheduleExpression = "* * * * *",
                Status = false
            });
            jobs.Add(new NodeJob()
            {
                Name = "Test2",
                ScheduleExpression = "* * * * *",
                Status = false
            });
            config.Jobs = jobs;

            return config;
        }
    }
}
