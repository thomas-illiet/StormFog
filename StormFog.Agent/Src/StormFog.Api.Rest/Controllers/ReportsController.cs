using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StormFog.Api.Rest.Controllers
{
    [ApiController]
    [Route("reports")]
    public class ReportsController : ControllerBase
    {
        public ReportsController()
        {

        }


        [HttpGet("hardware")]
        public string HardwareGet()
        {
            return "ok";
        }

        [HttpPost("hardware")]
        public string HardwarePost()
        {
            return "ok";
        }
    }
}
