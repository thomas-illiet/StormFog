using System;

namespace StormFog.Agent.Entity
{
    public class SFParameter
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
