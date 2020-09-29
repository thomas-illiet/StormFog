using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StormFog.Agent.Data;
using StormFog.Agent.Entity;
using StormFog.Agent.Shared.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StormFog.Agent.Providers
{
    /// <summary>
    /// Local configuration provider for the application.
    /// </summary>
    /// <seealso cref="ConfigurationProvider" />
    public class LocalConfigurationProvider : ConfigurationProvider
    {
        /// <summary>
        /// Gets or sets the name of the hosting environment.
        /// </summary>
        /// <value>
        /// The name of the hosting environment.
        /// </value>
        private readonly string EnvironmentName;

        /// <summary>
        /// The default values for settings
        /// </summary>
        private Lazy<Dictionary<string, string>> DefaultValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalConfigurationProvider"/> class.
        /// </summary>
        /// <param name="dbContextBuilderAction">The database context builder action.</param>
        /// <param name="environmentName">Name of the hosting environment.</param>
        public LocalConfigurationProvider(Action<DbContextOptionsBuilder> dbContextBuilderAction, string environmentName)
        {
            DbContextBuilderAction = dbContextBuilderAction;
            EnvironmentName = environmentName;
            InitDefaultValues(environmentName);
        }

        /// <summary>
        /// Gets the database context builder action.
        /// </summary>
        /// <value>
        /// The database context builder action.
        /// </value>
        private Action<DbContextOptionsBuilder> DbContextBuilderAction { get; }

        /// <summary>
        /// Loads (or reloads) the data for this provider.
        /// </summary>
        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            DbContextBuilderAction(builder);

            using (var dbContext = new DatabaseContext(builder.Options))
            {
                dbContext.Database.EnsureCreated();
                Data = !dbContext.Configurations.Any()
                    ? CreateAndSaveDefaultValues(dbContext)
                    : EnsureAllSync(dbContext.Configurations.ToDictionary(c => c.Key, c => c.Value), dbContext);
            }
        }

        /// <summary>
        /// Creates and save default values.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <returns>Data feeds for IOption.</returns>
        private IDictionary<string, string> CreateAndSaveDefaultValues(DatabaseContext dbContext)
        {
            AddInDb(DefaultValues.Value, dbContext);
            return DefaultValues.Value;
        }

        /// <summary>
        /// Add configurations to the database
        /// </summary>
        /// <param name="configurations"></param>
        /// <param name="dbContext">The database context.</param>
        private void AddInDb(IDictionary<string, string> configurations, DatabaseContext dbContext)
        {
            dbContext.Configurations.AddRange(configurations
                .Select(kvp => new SFParameter { Key = kvp.Key, Value = kvp.Value, CreatedAt = DateTime.UtcNow }));
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Ensures all values are present.
        /// </summary>
        /// <param name="dbValues">The database values.</param>
        /// <param name="dbContext">The database context.</param>
        /// <returns>Synced dictionary.</returns>
        private IDictionary<string, string> EnsureAllSync(IDictionary<string, string> dbValues, DatabaseContext dbContext)
        {
            // Find item which does not exist in DB values, for anything added later stage.
            var newItems = DefaultValues.Value.Where(item => !dbValues.Keys.Contains(item.Key))
                .ToDictionary(item => item.Key, item => item.Value);

            if (newItems.Any())
            {
                AddInDb(newItems, dbContext);
                return dbValues
                    .Concat(newItems.Select(item => new KeyValuePair<string, string>(item.Key, item.Value)))
                    .ToDictionary(item => item.Key, item => item.Value);
            }
            return dbValues;
        }

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        /// <param name="environmentName">Name of the environment.</param>
        private void InitDefaultValues(string environmentName)
        {
            // Can be done based on environment name.
            DefaultValues = new Lazy<Dictionary<string, string>>(() =>
            {
                return new Dictionary<string, string>
                {
                    { nameof(AppSetting.Version), Assembly.GetExecutingAssembly().GetName().Version.ToString() },
                    { nameof(AppSetting.Environment), environmentName },
                    { $"{nameof(AppSetting.ReportServer)}:{nameof(ReportServer.ServerURL)}", "https://stormfog.group.echonet/apis/rest" },
                    { $"{nameof(AppSetting.ReportServer)}:{nameof(ReportServer.RegistrationKey)}", Guid.Empty.ToString() },
                    { $"{nameof(AppSetting.ReportServer)}:{nameof(ReportServer.ProxyURL)}", string.Empty }
                };
            });
        }
    }

    /// <summary>
    /// Local Configuration source.
    /// </summary>
    /// <seealso cref="IConfigurationSource" />
    public class LocalConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// The database context builder action
        /// </summary>
        private readonly Action<DbContextOptionsBuilder> DbContextBuilderAction;

        /// <summary>
        /// The hosting environment name
        /// </summary>
        private readonly string EnvironmentName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalConfigurationSource"/> class.
        /// </summary>
        /// <param name="optionsAction">The options action.</param>
        /// <param name="environmentName">Name of the hosting environment.</param>
        public LocalConfigurationSource(Action<DbContextOptionsBuilder> optionsAction, string environmentName)
        {
            DbContextBuilderAction = optionsAction;
            EnvironmentName = environmentName;
        }

        /// <summary>
        /// Builds the <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" /> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" />.</param>
        /// <returns>
        /// An <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" />
        /// </returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new LocalConfigurationProvider(DbContextBuilderAction, EnvironmentName);
        }
    }

    /// <summary>
    /// Local configuration helper.
    /// </summary>
    public static class LocalConfigurationExtensions
    {
        /// <summary>
        /// Adds the entity framework configuration.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="setup">The setup.</param>
        /// <param name="environmentName">Name of the environment.</param>
        /// <returns><see cref="IConfigurationBuilder"/> after creation of EF config provider.</returns>
        public static IConfigurationBuilder AddLocalConfiguration(
                   this IConfigurationBuilder builder, Action<DbContextOptionsBuilder> setup, string environmentName)
        {
            return builder.Add(new LocalConfigurationSource(setup, environmentName));
        }

        /// <summary>
        /// Adds the configurations.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration root.</param>
        public static void AddLocalConfigurations(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<AppSetting>(configuration);
        }
    }
}