using CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using StormFog.Agent.BusinessLogic.Jobs;
using StormFog.Agent.BusinessLogic.Services;
using StormFog.Agent.Data;
using StormFog.Agent.Providers;
using System;
using System.Diagnostics;
using System.Reflection;

namespace StormFog.Agent
{
    public class Program
    {
        private static string DataDirectory { get;set; }
        private static string LogDirectory { get; set; }
        private static string LogFile { get; set; }
        private static string DatabaseFile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Initialize variables
            DataDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"StormFog");
            LogDirectory = System.IO.Path.Combine(DataDirectory, "Logs");
            LogFile = System.IO.Path.Combine(LogDirectory, "service.log");
            DatabaseFile = System.IO.Path.Combine(DataDirectory, "StormFog.Agent.db");

            // Parse and run the service if there is no error
            Parser.Default.ParseArguments<ProgramArguments>(args)
                          .WithParsed(opts => Service(opts));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arguments"></param>
        private static void Service(ProgramArguments arguments)
        {
            // Logger configurations
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(new LoggingLevelSwitch { MinimumLevel = arguments.LogLevel })
                .Enrich.FromLogContext()
                .WriteTo.File(LogFile, rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();

            // Show application informations
            Log.Information("Agent Version: {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Log.Information("Log directory: {0}", LogDirectory);
            Log.Information("Database path: {0}", DatabaseFile);

            // Start agent service
            try {
                Log.Information("Starting up the service");
                CreateHostBuilder().Build().Run();
                Log.Information("Ending service");
            }
            catch (Exception ex) { Log.Fatal(ex, "There was a problem starting the service"); }
            finally { Log.CloseAndFlush(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .UseWindowsService()
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Add a custom configuration provider to get the configuration through the database.
                    config.AddLocalConfiguration(options =>
                        options.UseSqlite($"Data Source={DatabaseFile}", assembly => assembly.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName)),
                        context.HostingEnvironment.EnvironmentName
                    );
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Define the configuration of the local database
                    services.AddDbContext<DatabaseContext>(options =>
                        options.UseSqlite($"Data Source={DatabaseFile}", assembly => assembly.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName))
                    );

                    // Register classes of service
                    services.AddTransient(typeof(ReportingService<>), typeof(ReportingService<>));

                    // Schedule jobs
                    //services.AddScheduleJob<UpdateParameterJob>(c => {
                    //    c.TimeZoneInfo = TimeZoneInfo.Local;
                    //    c.CronExpression = @"* * * * *";
                    //});

                    services.AddScheduleJobs(options =>options.UseSqlite($"Data Source={DatabaseFile}"));
                })
                .UseSerilog();
        }
    }
}
