using System;

namespace StormFog.Agent.Entity
{
    public class SFJob
    {
        public Guid Id { get; set; }
        public string Class { get; set; }
        public string CronExpression { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
