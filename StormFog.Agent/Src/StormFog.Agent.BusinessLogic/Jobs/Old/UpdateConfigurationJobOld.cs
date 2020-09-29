using Microsoft.Extensions.Logging;
using StormFog.Agent.BusinessLogic.Models;
using StormFog.Agent.BusinessLogic.Services;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace StormFog.Agent.BusinessLogic.Jobs
{
    public class UpdateConfigurationJobOld : ScheduleService
    {
        private readonly ILogger<UpdateConfigurationJobOld> _logger;
        private readonly ReportingService<HardwareReport> _hardWareReport;
        public UpdateConfigurationJobOld(
            ReportingService<HardwareReport> hardWareReport,
            IScheduleConfig<UpdateConfigurationJobOld> config,
            ILogger<UpdateConfigurationJobOld> logger)
            : base(config.CronExpression, config.TimeZoneInfo)

        { 
            _hardWareReport = hardWareReport;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateConfigurationJob starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateConfigurationJob is working.");

            var report = _hardWareReport.GetReport();

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateConfigurationJob is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
