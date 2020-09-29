using CommandLine;
using Serilog.Events;
using StormFog.Agent.Shared.Models;
using System;

namespace StormFog.Agent
{
    internal sealed class ProgramArguments
    {
        [Option('l', "logLevel", HelpText = "Specifies the meaning and relative importance of a log event. (Verbose|Debug|Information|Warning|Error|Fatal)")]
        public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;

        [Option('m', "mode", Default = ExecutionMode.User, HelpText = "Define the execution context of the agent. (System|User)")]
        public ExecutionMode Mode { get; set; }
    }
}