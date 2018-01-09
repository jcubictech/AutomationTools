namespace SS_PricePushKickOff
{
    using SenStaySync;
    using SenStaySync.Data;
    using SenStaySync.Tools;
    using SS_AirBnbPricePush;
    using SS_StreamlinePricePush;
    using System;
    using System.IO;
    using System.Security.Permissions;

    class Program
    {
        static void Main(string[] args)
        {
            Run();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Run()
        { 
            string filePath = Config.I.PricingPushFile;
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Path = Path.GetDirectoryName(filePath);

            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
           
            watcher.Filter = Path.GetFileName(filePath);

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnCreated);
           
            // Begin watching.
            watcher.EnableRaisingEvents = true;

            // Wait for the user to quit the program.
            Console.WriteLine("Type \'Exit\' to close the application.");
            while (Console.ReadLine() != "Exit") ;
        }

        /// <summary>
        /// On changed event.
        /// </summary>
        /// <param name="source">Source from where the event originated.</param>
        /// <param name="e">Event.</param>
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            var fsw = source as FileSystemWatcher;
            try
            {
                fsw.EnableRaisingEvents = false;

                ProcessPricePush();

                Console.Clear();
                Console.WriteLine("Type \'Exit\' to close the application.");
            }

            finally
            {
                fsw.EnableRaisingEvents = true;
            }
          
        }

        /// <summary>
        /// On created event
        /// </summary>
        /// <param name="source">Source from where the event originated.</param>
        /// <param name="e">Event.</param>
        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            var fsw = source as FileSystemWatcher;
            try
            {
                fsw.EnableRaisingEvents = false;
                ProcessPricePush();

                Console.Clear();
                Console.WriteLine("Type \'Exit\' to close the application.");
            }
            finally
            {
                fsw.EnableRaisingEvents = true;
            }
        }

        public static void ProcessPricePush()
        {
            N.Note("Price input file has been updated");
            

            DateTime processStartedAt = DateTime.UtcNow;
            N.Note("Process started at :" + processStartedAt.ToString());

            var pricePushLogger = new OperationsJsonLogger<PricePushResult>(Config.I.PricingPushAttemptsLogFile, processStartedAt);
            var properties = PricingPushCSVReader.ReadPricingData(pricePushLogger);
            PricingPushNotification pricingPushNotification = new PricingPushNotification();

            if (properties != null)
            {
                pricingPushNotification.SendInitialNotification(processStartedAt);

                if (Config.I.PushPriceToAirBnb)
                {
                    if (!string.IsNullOrEmpty(properties[0].AirbnbId))
                    {
                        N.Note("Starting the price push to AirBnb");

                        pricePushLogger.Log(new PricePushResult(Channel.AirBnb, PricePushLogArea.ProcessStarted, PricingPushLogType.Information, "Pricing push process started at :" + DateTime.UtcNow.ToString()));

                        PricingPushCSVReader.LogPropertiesForAirBnb(properties, pricePushLogger);
                        // Kick start the Airbnb process.
                        PricePush.PushPriceToAirBnb(processStartedAt, pricePushLogger, properties);

                        N.Note("Pricing push to AirBnb is finished");
                        // Read the log files and send the email with respect to Airbnb.

                        N.Note("Send notification email for AirBnb");

                        pricingPushNotification.SendNotification(processStartedAt);
                    }
                }

                if (Config.I.PushPriceToStreamLine)
                {
                    if (!string.IsNullOrEmpty(properties[0].StreamLineHomeId))
                    {
                        N.Note("Starting the price push to Streamline");

                        pricePushLogger.Log(new PricePushResult(Channel.StreamLine, PricePushLogArea.ProcessStarted, PricingPushLogType.Information, "Pricing push process started at :" + DateTime.UtcNow.ToString()));

                        PricingPushCSVReader.LogPropertiesForStreamLine(properties, pricePushLogger);
                        // Kick start the Stream Line process
                        StreamlinePricePush.PushPriceToStreamLine(processStartedAt, pricePushLogger, properties);

                        N.Note("Pricing push to streamline is finished");

                        N.Note("Send notification email for Streamline");

                        pricingPushNotification.SendNotificationToStreamLine(processStartedAt);
                    }
                }
            }
        }
    }
}
