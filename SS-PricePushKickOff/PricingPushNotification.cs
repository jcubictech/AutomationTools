using System;
using System.Linq;
using SenStaySync;
using SenStaySync.Data;
using SenStaySync.Tools;
using SenStaySync.PageProcesser.AirBnb;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System.IO;
using System.Reflection;

namespace SS_PricePushKickOff
{
    public class PricingPushNotification
    {

        /// <summary>
        ///  Send notification email when the process starts
        /// </summary>
        /// <param name="processStartedDateTime">Process started time</param>
        public void SendInitialNotification(DateTime processStartedDateTime)
        {
            var config = new TemplateServiceConfiguration
            {
                BaseTemplateType = typeof(HtmlSupportTemplateBase<>),
                DisableTempFileLocking = true,
                CachingProvider = new DefaultCachingProvider(t => { })
            };

            bool pushPriceToAirBnb = Config.I.PushPriceToAirBnb;
            bool pushPriceToStreamLine = Config.I.PushPriceToStreamLine;

            var templatePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);                                                                                  
            var template = File.ReadAllText(Path.Combine(templatePath, "Templates/PricingPushInitialNotificationTemplate.cshtml"));
         
            using (var service = RazorEngineService.Create(config))
            {
                var body = service.RunCompile(template, "PricingPushInitialNotificationTemplate", null, new
                {
                    ProcessStartedAt = processStartedDateTime.ToString("yyyy-M-d HH:mm tt"),
                    PushToAirbnb = pushPriceToAirBnb,
                    PushToStreamline = pushPriceToStreamLine
                });

                EmailNotification.Send(Config.I.NotificationsEmail, "Pricing Push Initialized", body);
            }
        }

        /// <summary>
        ///  Send notification mail with the results of price push to AirBnb.
        /// </summary>
        /// <param name="logWrittenDateTime">Log written date and time.</param>
        public void SendNotification(DateTime logWrittenDateTime)
        {
            var config = new TemplateServiceConfiguration
            {
                BaseTemplateType = typeof(HtmlSupportTemplateBase<>),
                DisableTempFileLocking = true,
                CachingProvider = new DefaultCachingProvider(t => { })
            };

            var pricingPushLogger = new OperationsJsonLogger<PricePushResult>(Config.I.PricingPushAttemptsLogFile);
            var pricingPushlogs = pricingPushLogger.GetLogRecordsForExactDateTime(logWrittenDateTime);
            var airBnbLogs = pricingPushlogs.Where(p => p.Channel == Channel.AirBnb || p.Channel == Channel.Common);
            var templatePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var template = File.ReadAllText(Path.Combine(templatePath, "Templates/PricingPushNotificationTemplate.cshtml"));

            using (var service = RazorEngineService.Create(config))
            {
                var body = service.RunCompile(template, "PricingPushNotificationTemplate", null, new {
                    ProcessStartedAt = logWrittenDateTime.ToString("yyyy-M-d HH:mm tt"),
                    ErrorsCount = airBnbLogs.Where(p => p.LogType == PricingPushLogType.Error).Count(),
                    ParsingCSV = airBnbLogs.Where(p => ((p.LogArea == PricePushLogArea.ParsingCSV || p.LogArea == PricePushLogArea.CSVFileNotFound) && p.LogType == PricingPushLogType.Error)),
                    PropertyDetails = airBnbLogs.Where(p=> p.LogArea == PricePushLogArea.ParsingCSV && p.LogType == PricingPushLogType.Information),
                    LoginDetails = airBnbLogs.Where(p=> p.LogArea == PricePushLogArea.Login && p.LogType == PricingPushLogType.Information),
                    LoginErrors = airBnbLogs.Where(p => p.LogArea == PricePushLogArea.Login && p.LogType == PricingPushLogType.Error),
                    PropertyErrors = airBnbLogs.Where(p=> p.LogArea == PricePushLogArea.Property && p.LogType == PricingPushLogType.Error),
                    PriceUpdationErrors = airBnbLogs.Where(p=>p.LogArea == PricePushLogArea.PriceUpdate &&  p.LogType == PricingPushLogType.Error)
                });

                EmailNotification.Send(Config.I.PricingPushNotificationsEmail, "AirBnb Pricing Push Report", body);
            }

        }

        /// <summary>
        /// Send notification mail with the results of price push to Streamline.
        /// </summary>
        /// <param name="logWrittenDateTime">Log written date and time.</param>
        public void SendNotificationToStreamLine(DateTime logWrittenDateTime)
        {
            var config = new TemplateServiceConfiguration
            {
                BaseTemplateType = typeof(HtmlSupportTemplateBase<>),
                DisableTempFileLocking = true,
                CachingProvider = new DefaultCachingProvider(t => { })
            };

            var pricingPushLogger = new OperationsJsonLogger<PricePushResult>(Config.I.PricingPushAttemptsLogFile);
            var pricingPushlogs = pricingPushLogger.GetLogRecordsForExactDateTime(logWrittenDateTime);
            var streamLineLogs = pricingPushlogs.Where(p => p.Channel == Channel.StreamLine || p.Channel == Channel.Common);
            var templatePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var template = File.ReadAllText(Path.Combine(templatePath, "Templates/StreamLinePricingPushNotificationTemplate.cshtml"));

            using (var service = RazorEngineService.Create(config))
            {
                var body = service.RunCompile(template, "StreamLinePricingPushNotificationTemplate", null, new
                {
                    ProcessStartedAt = logWrittenDateTime.ToString("yyyy-M-d HH:mm tt"),
                    ErrorsCount = streamLineLogs.Where(p => p.LogType == PricingPushLogType.Error).Count(),
                    ParsingCSV = streamLineLogs.Where(p => ((p.LogArea == PricePushLogArea.ParsingCSV || p.LogArea == PricePushLogArea.CSVFileNotFound) && p.LogType == PricingPushLogType.Error)),
                    PropertyDetails = streamLineLogs.Where(p => p.LogArea == PricePushLogArea.ParsingCSV && p.LogType == PricingPushLogType.Information),
                    LoginDetails = streamLineLogs.Where(p => p.LogArea == PricePushLogArea.Login && p.LogType == PricingPushLogType.Information),
                    LoginErrors =  streamLineLogs.Where(p => p.LogArea == PricePushLogArea.Login && p.LogType == PricingPushLogType.Error),
                    SeasonGroupErrors = streamLineLogs.Where(p => ((p.LogArea == PricePushLogArea.SeasonGroup || p.LogArea == PricePushLogArea.ChangeSeasonGroup) && p.LogType == PricingPushLogType.Error)),
                    SeasonErrors = streamLineLogs.Where(p => p.LogArea == PricePushLogArea.Season && p.LogType == PricingPushLogType.Error),
                    PropertyErrors = streamLineLogs.Where(p => p.LogArea == PricePushLogArea.Property && p.LogType == PricingPushLogType.Error),
                    PriceUpdationErrors = streamLineLogs.Where(p => p.LogArea == PricePushLogArea.PriceUpdate && p.LogType == PricingPushLogType.Error),
                });

                EmailNotification.Send(Config.I.PricingPushNotificationsEmail, "Streamline Pricing Push Report", body);
            }
        }
    }
}
