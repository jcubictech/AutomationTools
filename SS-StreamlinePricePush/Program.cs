using SenStaySync;
using SenStaySync.Data;
using SenStaySync.Tools;
using System;

namespace SS_StreamlinePricePush
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Begin pricing push to StreamLine");

            var pricePushLogger = new OperationsJsonLogger<PricePushResult>(Config.I.PricingPushAttemptsLogFile, DateTime.UtcNow);
            var properties = PricingPushCSVReader.ReadPricingData(pricePushLogger);

            StreamlinePricePush.PushPriceToStreamLine(DateTime.Now, pricePushLogger, properties);

            Console.WriteLine("End pricing push to StreamLine");
        }  
    }
}
