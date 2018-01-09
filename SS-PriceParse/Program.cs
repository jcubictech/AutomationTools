namespace SS_PriceParse
{
    using System;
    using SenStaySync;
    using SenStaySync.Prices;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pricing Parsing 2016-01-14");
            PriceParsing.ParseFolder(Config.I.PriceSourceDirectory);
            Console.WriteLine("END");
        }
    }
}