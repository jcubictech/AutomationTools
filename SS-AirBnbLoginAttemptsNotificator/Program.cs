namespace SS_AirBnbLoginAttemptsNotificator
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using RazorEngine.Configuration;
    using RazorEngine.Templating;
    using SenStaySync;
    using SenStaySync.Data;
    using SenStaySync.Tools;

    class Program
    {
        static void Main(string[] args)
        {
            var config = new TemplateServiceConfiguration
            {
                DisableTempFileLocking = true,
                CachingProvider = new DefaultCachingProvider(t => { })
            };

            var logDate = DateTime.UtcNow.AddDays(-1);

            var loginAttemptsLogger = new OperationsJsonLogger<LoginAttempt>(Config.I.LoginAttemptsLogFile);
            var statistics = loginAttemptsLogger.GetStatistics(logDate);

            var passSyncAttemptsLogger = new OperationsJsonLogger<PasswordSyncError>(Config.I.PasswordSyncLogFile);
            var errors = passSyncAttemptsLogger.GetLogRecords(logDate)
                .Select(x => new
                            {
                                Description = x.ErrorType.ToDescription(), x.Date
                            });

            var templatePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var template = File.ReadAllText(Path.Combine(templatePath, "Templates/NotificationTemplate.cshtml"));

            using (var service = RazorEngineService.Create(config))
            {
                var body = service.RunCompile(template, "NotificationEmailTemplate", null, new
                {
                    statistics.ErrorsCount,
                    LoginAttempts = statistics.ErrorLogins,
                    TotalAccountsProcessed = statistics.LoginCount,
                    PasswordSyncErrors = errors,
                    ProxyErrors = statistics.InvalidProxies
                });

                EmailNotification.Send(Config.I.NotificationsEmail, "Airbnb Export report", body);
            }

            passSyncAttemptsLogger.Clear();
            passSyncAttemptsLogger.Clear();
        }
    }
}
