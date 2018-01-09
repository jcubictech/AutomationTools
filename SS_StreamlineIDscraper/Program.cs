namespace SS_StreamlineIDscraper
{
    using System;
    using SenStaySync;
    using SenStaySync.Data;
    using SenStaySync.PageProcesser.Streamline;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SenStay Streamline Scraper");

            Config.I.UseProxy = true;
            Config.I.RegularWaitSeconds = 30;

            Temp.TouchDirectory(Config.I.ImagesFolder);
            var account = new StreamlineAccount
            {
                Login = "devtest1",
                Password = "DevTest11"
            };

            using (var driver = SeleniumFactory.GetFirefoxDriver())
            {
                try
                {
                    StreamlineLogin.Process(driver, account);
                    /////////////KILLL IT
                    StreamlineSeasonProcesser.ScrapeSeasons(driver, "21713");
                    return;
                    /////////////KILLL IT

                    if (args.Length > 0)
                    {
                        if (args[0] == "seasons")
                        {
                            //StreamlineSeasonProcesser.ScrapeSeasons(driver, "20250");
                            //StreamlineSeasonProcesser.ScrapeSeasons(driver, "20826");
                            StreamlineSeasonProcesser.ScrapeSeasons(driver, "21713");
                        }

                        if (args[0] == "additional")
                        {
                            StreamlineScrapeUnits.Process(driver, true);
                        }
                    }
                    else
                    {
                        StreamlineScrapeUnits.Process(driver, false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex + "");
                }
            }
        }
    }
}