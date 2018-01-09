namespace SS_PriceSync
{
    using System;
    using SenStaySync.Prices;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pricing sync 2016-01-14");
            DailyPriceProcesser.RunCommands();
            //Console.ReadLine();

            /*
            //var unitId = "LA002";
            var unitId = "LA081";
            //var unitId = "TEST001";
            //var csvFile = @"D:\Src\SenStay\Data\2016-01-07 LA002.csv";
            var csvFile = @"D:\Src\SenStay\Data\2016-01-07 LA081.csv";
            var data = SenStaySync.Tools.CSVReader.Read2columnCSV(csvFile);

            var map = new PriceMap();
            foreach (var d in data)
            {
                var dateStr = d.Item1;
                var priceStr = d.Item2;

                DateTime date = DateTime.UtcNow;
                int Price = 0;

                if (!DateTime.TryParse(dateStr, out date))
                {
                    continue;
                }

                if (!Int32.TryParse(priceStr, out Price))
                {
                    continue;
                }

                
                map.Add(unitId, date.Month, date.Day, Price, DateTime.UtcNow);
            }

            Config.I.UseProxy = false;

            
            //var map = new PriceMap();
            //map.Add("TEST001", 1, 1, 1001, DateTime.UtcNow);
            //map.Add("TEST001", 1, 2, 1002, DateTime.UtcNow);
            //map.Add("TEST001", 1, 3, 1003, DateTime.UtcNow);
            //map.Add("TEST001", 1, 4, 1004, DateTime.UtcNow);
            //map.Add("TEST001", 1, 5, 1005, DateTime.UtcNow);
            

            var Seasons = StreamlineSeasonGroup.Load();
            var Units = StreamlinePropertyCollection.Load().CreateIndex();

            var prices = map.GetPricesByUnit(unitId, Units, Seasons);


            //Config.UseProxy = true;
            var Account = new SenStaySync.Data.StreamlineAccount()
            {
                Login = "devtest1",
                Password = "DevTest11"
            };

            var Driver = SenStaySync.SeleniumFactory.GetFirefoxDriver();
            //try {

            var UnitInfo = Units.GetBySenStayID(unitId);
            //"54248"

            SenStaySync.PageProcesser.Streamline.StreamlineLogin.Process(Driver, Account, false);
            SenStaySync.PageProcesser.Streamline.StreamlineDailyPrice.SetPricesForUnit(Driver, UnitInfo.StreamlineEditID, prices.GetSeasonPriceList(), false);
            

            //} catch { Driver.Quit(); }

            //Driver.Quit();
            //*/
        }
    }
}