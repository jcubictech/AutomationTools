namespace SS_PriceSyncAirbnb
{
    using System;
    using SenStaySync;
    using SenStaySync.Data;
    using SenStaySync.Data.Streamline;
    using SenStaySync.PageProcesser.AirBnb;
    using SenStaySync.Prices;
    using SenStaySync.Tools;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pricing sync for AirBnb");
            Config.I.UseProxy = true;

            AirBnbAccounts.Load();
            var roomsMap = AirBnbRoomMap.Load();
            var streamlineCollection = StreamlinePropertyCollection.Load();
            PropertyMap.SetMap(roomsMap, streamlineCollection);

            var property = PropertyMap.GetBySenstayID("LA029");
            var proxyNum = 0;

            var airAccount = AirBnbAccounts.GetAccountByEmail(property.AirbnbAccountEmail);

            var email = airAccount.Email;
            N.Note("Processing account " + email + " with proxy " + airAccount.ProxyAddress[proxyNum]);
            var profile = SeleniumProxy.GetFirefoxProfileWithProxy(airAccount.ProxyAddress, proxyNum);

            using (var driver = SeleniumFactory.GetFirefoxDriver(profile))
            {
                var signInSucceed = AirBnbLogin.Process(driver, airAccount);
                if (signInSucceed.Status != AirBnbLoginStatus.Sucess)
                {
                    N.Note("Password for " + email + " is wrong or account unreachable");
                    try
                    {
                        driver.Quit();
                    }
                    catch
                    {
                        // ignored
                    }
                }
                AirBnbCalendar.SetPricesForUnit(driver, property, AirbnbUnitDailyPriceList.Load());
            }
        }
    }
}