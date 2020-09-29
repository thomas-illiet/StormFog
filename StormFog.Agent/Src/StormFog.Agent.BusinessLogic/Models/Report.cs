using System;
using System.Collections.Generic;
using System.Text;

namespace StormFog.Agent.BusinessLogic.Models
{
    public class Report<TEntity>
    {
        public string HostName { get; set; }
        public DateTime Date { get; set; }
        public TEntity Data { get; set; }
    }
}
