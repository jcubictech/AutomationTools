//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Senstay">
//     Sentay.
// </copyright>
//-----------------------------------------------------------------------

namespace SS_AirBnbPricePush
{
    using SenStaySync;
    using SenStaySync.Data;
    using SenStaySync.Tools;
    using System;

    /// <summary>
    /// AirBnb price push.
    /// </summary>
    public class Program
    { 
        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">Arguments which can be passed into the program.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Begin pricing push to AirBnb");

            var pricePushLogger = new OperationsJsonLogger<PricePushResult>(Config.I.PricingPushAttemptsLogFile, DateTime.UtcNow);
            var properties = PricingPushCSVReader.ReadPricingData(pricePushLogger);
            PricePush.PushPriceToAirBnb(DateTime.UtcNow, pricePushLogger, properties);

            Console.WriteLine("End pricing push to AirBnb");
        }
    }    
}
