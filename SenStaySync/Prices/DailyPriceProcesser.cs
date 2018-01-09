namespace SenStaySync.Prices
{
    using System;
    using Data;
    using OpenQA.Selenium;
    using PageProcesser.Streamline;

    public static class DailyPriceProcesser
    {
        public static void RunCommands()
        {
            try
            {
                //Price
                var Account = new StreamlineAccount
                {
                    Login = "devtest1",
                    Password = "DevTest11"
                };

                var scripts = PriceScript.LoadAllScripts();
                foreach (var script in scripts)
                {
                    IWebDriver driver = null;
                    try
                    {
                        driver = SeleniumFactory.GetFirefoxDriver();
                        driver.JustWait(1);
                        N.Note("Run PriceScript for " + script.UnitId + " generated at " + script.Generated);
                        StreamlineLogin.Process(driver, Account, false);
                        driver.Manage().Window.Maximize();
                        StreamlineDailyPrice.RunPriceScript(driver, script);
                        driver.JustWait(1);
                        driver.Quit();
                        script.Archive();
                    }
                    catch (Exception e)
                    {
                        N.Note(e + "");
                    }
                    finally
                    {
                        if (driver != null) driver.Quit();
                    }
                }
            }
            catch
            {
            }
        }
    }
}