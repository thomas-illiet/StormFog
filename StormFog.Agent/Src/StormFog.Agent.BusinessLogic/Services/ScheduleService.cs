using Cronos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StormFog.Agent.Data;
using StormFog.Agent.Entity;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace StormFog.Agent.BusinessLogic.Services
{
    public abstract class ScheduleService : IHostedService, IDisposable
    {
        private System.Timers.Timer _timer;
        private readonly CronExpression _expression;
        private readonly TimeZoneInfo _timeZoneInfo;

        protected ScheduleService(string cronExpression, TimeZoneInfo timeZoneInfo)
        {
            _expression = CronExpression.Parse(cronExpression);
            _timeZoneInfo = timeZoneInfo;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            await ScheduleJob(cancellationToken);
        }

        protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
        {
            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds <= 0)   // prevent non-positive values from being passed into Timer
                {
                    await ScheduleJob(cancellationToken);
                }
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();  // reset and dispose timer
                    _timer = null;

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await DoWork(cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await ScheduleJob(cancellationToken);    // reschedule next
                    }
                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }

        public virtual async Task DoWork(CancellationToken cancellationToken)
        {
            await Task.Delay(5000, cancellationToken);  // do the work
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();
            await Task.CompletedTask;
        }

        public virtual void Dispose()
        {
            _timer?.Dispose();
        }
    }

    public interface IScheduleConfig<T>
    {
        string CronExpression { get; set; }
        TimeZoneInfo TimeZoneInfo { get; set; }
    }

    public class ScheduleConfig<T> : IScheduleConfig<T>
    {
        public string CronExpression { get; set; }
        public TimeZoneInfo TimeZoneInfo { get; set; }
    }

    public static class ScheduleExtensions
    {
        public static IServiceCollection AddScheduleJobs(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextBuilderAction)
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            dbContextBuilderAction(builder);

            using (var dbContext = new DatabaseContext(builder.Options))
            {
                var jobEntities = dbContext.Set<SFJob>().ToList();



                foreach (var jobEntity in jobEntities)
                {
                    // Make parameter
                    Type myType = typeof(ScheduleConfig<>).MakeGenericType(Type.GetType(jobEntity.Class));
                    dynamic instance = Activator.CreateInstance(myType);

                    var cronExpression = instance.GetType().GetProperty("CronExpression");
                    cronExpression.SetValue(instance, @"* * * * *", null);

                    var timeZoneInfo = instance.GetType().GetProperty("TimeZoneInfo");
                    timeZoneInfo.SetValue(instance, TimeZoneInfo.Local, null);

                    // Call ScheduleJob Method
                    MethodInfo method = typeof(ScheduleExtensions).GetMethod(nameof(ScheduleExtensions.AddScheduleJob));
                    MethodInfo generic = method.MakeGenericMethod(Type.GetType(jobEntity.Class));
                    generic.Invoke(null, instance);
                }
            }

            return services;
        }

        public static IServiceCollection AddScheduleJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : ScheduleService
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), @"Please provide Schedule Configurations.");
            }
            var config = new ScheduleConfig<T>();
            options.Invoke(config);
            if (string.IsNullOrWhiteSpace(config.CronExpression))
            {
                throw new ArgumentNullException(nameof(ScheduleConfig<T>.CronExpression), @"Empty Cron Expression is not allowed.");
            }

            services.AddSingleton<IScheduleConfig<T>>(config);
            services.AddHostedService<T>();
            return services;
        }
    }
}
