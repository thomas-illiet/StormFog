using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace StormFog.Agent.BusinessLogic.Services
{
    public class ReportingService<TEntity>
    {
        private readonly ILogger<ReportingService<TEntity>> _logger;

        public ReportingService(ILogger<ReportingService<TEntity>> logger)
        {
            _logger = logger;
        }

        public TEntity GetReport()
        {
            var report = Activator.CreateInstance<TEntity>();
            foreach (var property in typeof(TEntity).GetProperties())
            {
                // Find method
                var propertyType = Activator.CreateInstance(property.PropertyType);
                var method = property.PropertyType.GetMethods().FirstOrDefault
                                   (method => method.Name == "Retrieve"
                                    && method.GetParameters().Count() == 0);
                if (method == null)
                    throw new Exception("Unable to find retrieve method");

                // Execute method to get report
                var result = method.Invoke(propertyType, null);

                // 
                var propertyInfo = report.GetType().GetProperty(property.Name);
                propertyInfo.SetValue(report, result, null);
            }

            return report;
        }

        public void Publish()
        {

        }


    }
}
