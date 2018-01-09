namespace SS_AirBnb2Streamline
{
    using SenStaySync;
    using SenStaySync.Data;
    using SenStaySync.PageProcesser.AirBnb;
    using SenStaySync.Test.Scripts;
    using SenStaySync.Tools;

    class Program
    {
        static void Main(string[] args)
        {
            //Config.I.UseProxy = false;
            Config.I.UseProxy = true;

            DataInit.Init();

            //debug();
            //return;

            //var Account = Data.AirBnbAccounts.GetAccountByEmail("oliverrentals1@gmail.com");
            var account = AirBnbAccounts.GetAccountByEmail("sashaairbnb1@gmail.com");
            var profile = SeleniumProxy.GetFirefoxProfileWithProxy(account.ProxyAddress);
            var driver = SeleniumFactory.GetFirefoxDriver(profile);
            AirBnbLogin.Process(driver, account);

            var items = AirBnbScrapeReservation.GetAllReservationItems(driver);
            AirBnbScrapeReservation.GetFullReservationInfo(items[0], driver, account);

            driver.Quit();
        }

        static void debug()
        {
            //SenStaySync.Test.Scripts.Scrape1.ProcessProduction();
            //SenStaySync.Test.Scripts.Scrape1.Process();
            StreamlineQuickBook1.ProductionProcess();

            //SenStaySync.Test.Scripts.StreamlineLogin1.Process();
            //SenStaySync.Test.Scripts.StreamlineLogin1.ProductionProcess();
        }
    }
}