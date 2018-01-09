namespace SS_AirBnbExport
{
    using log4net;
    using log4net.Appender;
    using log4net.Core;
    using log4net.Layout;
    using log4net.Repository.Hierarchy;

    public class Logger
    {
        public static void Setup()
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository();

            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%date %-5level %logger - %message%newline"
            };

            patternLayout.ActivateOptions();

            var roller = new RollingFileAppender
            {
                AppendToFile = true,
                DatePattern = "dd.MM.yyyy'-events.log'",
                File = @"Logs\",
                Layout = patternLayout,
                RollingStyle = RollingFileAppender.RollingMode.Date,
                StaticLogFileName = false
            };

            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            var memory = new MemoryAppender();
            memory.ActivateOptions();

            hierarchy.Root.AddAppender(memory);
            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;
        }
    }
}