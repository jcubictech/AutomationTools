namespace SS_AirBnbScraper
{
    using System;
    using SenStaySync;
    using SenStaySync.Data;
    using SenStaySync.PageProcesser.AirBnb;
    using SenStaySync.Tools;

    class Program
    {
        static void Main(string[] args)
        {
            Config.I.UseProxy = true;

            AirBnbAccounts.Load();

            var map = AirBnbRoomMap.Load();

            foreach (var airAccount in AirBnbAccounts.Accounts)
            {
                if (map.GetAccount(airAccount.Email) != null)
                {
                    N.Note(airAccount.Email + " skiped");
                    continue;
                }

                for (var proxyNum = 0; proxyNum < 1; proxyNum++)
                {
                    var email = airAccount.Email;
                    N.Note("Processing account " + email + " with proxy " + airAccount.ProxyAddress[proxyNum]);
                    var profile = SeleniumProxy.GetFirefoxProfileWithProxy(airAccount.ProxyAddress, proxyNum);
                    var driver = SeleniumFactory.GetFirefoxDriver(profile);
                    try
                    {
                        var signInSucceed = AirBnbLogin.Process(driver, airAccount);
                        if (signInSucceed.Status == AirBnbLoginStatus.Failed)
                        {
                            N.Note("Password for " + email + " is wrong or account unreachable");
                            try
                            {
                                driver.Quit();
                            }
                            catch
                            {
                            }
                            break;
                        }
                        var item = AirBnbScrapeRooms.GetRoomList(driver, airAccount);
                        map.List.Add(item);

                        driver.Quit();
                    }
                    catch (Exception e)
                    {
                        N.Note("Error " + airAccount.Email + " : " + e + "".Substring(0, 100) + "...");
                        driver.Quit();
                    }
                }
            }
            map.Save();
        }
    }
}