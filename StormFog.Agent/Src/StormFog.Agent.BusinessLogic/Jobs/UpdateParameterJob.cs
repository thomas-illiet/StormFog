using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StormFog.Agent.BusinessLogic.Services;
using StormFog.Agent.Data;
using StormFog.Agent.Entity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StormFog.Agent.BusinessLogic.Jobs
{
    public class UpdateParameterJob : ScheduleService
    {
        private readonly ILogger<UpdateParameterJob> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public UpdateParameterJob(
            IScheduleConfig<UpdateParameterJob> config,
            ILogger<UpdateParameterJob> logger,
            IServiceScopeFactory scopeFactory)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateConfigurationJob is working.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                _logger.LogInformation("Get All");

                var localParameterEntities = dbContext.Set<SFParameter>().ToList();
                var localParameterDict = localParameterEntities.ToDictionary(x => x.Id);

                _logger.LogInformation("Add in list");

                var parameterEntity = new SFParameter()
                {
                    Id = Guid.Parse("22e9693e-cf2d-4b19-82c5-ea02594447ea"),
                    Key = "test:tt:tt",
                    Value = "prout",
                    CreatedAt = DateTime.UtcNow
                    
                };
                //localParameterEntities.Add(parameterEntity);
                dbContext.Set<SFParameter>().Add(parameterEntity);

                _logger.LogInformation("Update Database");
                dbContext.SaveChanges();
                

            }



            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
