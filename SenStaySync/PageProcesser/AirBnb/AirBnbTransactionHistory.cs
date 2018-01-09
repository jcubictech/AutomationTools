﻿namespace SenStaySync.PageProcesser.AirBnb
{
    using System;
    using Data;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.Support.UI;

    public static class AirBnbTransactionHistory
    {
        public static string url = @"https://www.airbnb.com/users/transaction_history";

        public static void PrepareProfileForDownloading(FirefoxProfile firefoxProfile)
        {
            firefoxProfile.SetPreference("browser.download.folderList", 2);
            //try
            //{
            //    firefoxProfile.SetPreference("browser.download.manager.showWhenStarting", false);
            //}
            //catch (Exception ex)
            //{
            //    // ignored
            //}
            firefoxProfile.SetPreference("browser.download.dir", Config.I.TempDirectory);
            firefoxProfile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "text/csv");
        }

        public static bool DownloadCompletedTransaction(IWebDriver driver, AirBnbAccount account, int Year)
        {
            driver.GoTo(url);
            var _wait = new WebDriverWait(driver, new TimeSpan(0, 0, Config.I.RegularWaitSeconds*2));

            // ждем пока загрузится
            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".transaction-table")));

            driver.JustWait(1);

            try
            {
                //class="payout-method" => value=""
                var payoutMethod = new SelectElement(driver.FindElement(By.CssSelector(".payout-method")));
                payoutMethod.SelectByValue("");
            }
            catch
            {
            }


            try
            {
                //class="payout-listing" => value=""
                var payoutListing = new SelectElement(driver.FindElement(By.CssSelector(".payout-listing")));
                payoutListing.SelectByValue("");
            }
            catch
            {
            }


            try
            {
                //class="payout-year" => current year
                var payoutYear = new SelectElement(driver.FindElement(By.CssSelector(".payout-year")));
                payoutYear.SelectByValue(Year.ToString());
            }
            catch
            {
            }


            //class="payout-start-month" => 1
            var payoutStartMonth = new SelectElement(driver.FindElement(By.CssSelector(".payout-start-month")));
            payoutStartMonth.SelectByValue("1");


            try
            {
                //class="payout-end-month" => 12
                var payoutEndMonth = new SelectElement(driver.FindElement(By.CssSelector(".payout-end-month")));
                payoutEndMonth.SelectByValue("12");
            }
            catch
            {
            }

            try
            {
                //class="export-csv-link" => click to download
                var AExportCsvLink = driver.FindElement(By.CssSelector("a.export-csv-link"));
                AExportCsvLink.Click();
                driver.JustWait(10);
            }
            catch
            {
                return false;
            }

            return true;
            //
        }

        public static bool DownloadFutureTransaction(IWebDriver driver, AirBnbAccount account, int Year)
        {
            // aria-controls="future-transactions"

            driver.GoTo(url);
            var _wait = new WebDriverWait(driver, new TimeSpan(0, 0, Config.I.RegularWaitSeconds*2));

            // ждем пока загрузится
            _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[aria-controls=\"future-transactions\"]")));

            driver.JustWait(1);

            try
            {
                var AFutureTransactions = driver.FindElement(By.CssSelector("a[aria-controls=\"future-transactions\"]"));
                AFutureTransactions.Click();
                _wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".transaction-table")));
                driver.JustWait(1);
            }
            catch
            {
                return false;
            }


            try
            {
                //class="payout-method" => value=""
                var payoutMethod = new SelectElement(driver.FindElement(By.CssSelector(".payout-method")));
                payoutMethod.SelectByValue("");
            }
            catch
            {
            }


            try
            {
                //class="payout-listing" => value=""
                var payoutListing = new SelectElement(driver.FindElement(By.CssSelector(".payout-listing")));
                payoutListing.SelectByValue("");
            }
            catch
            {
            }


            try
            {
                //class="payout-year" => current year
                var payoutYear = new SelectElement(driver.FindElement(By.CssSelector(".payout-year")));
                payoutYear.SelectByValue(Year.ToString());
            }
            catch
            {
            }


            try
            {
                //class="payout-start-month" => 1
                var payoutStartMonth = new SelectElement(driver.FindElement(By.CssSelector(".payout-start-month")));
                payoutStartMonth.SelectByValue("1");
            }
            catch
            {
            }


            try
            {
                //class="payout-end-month" => 12
                var payoutEndMonth = new SelectElement(driver.FindElement(By.CssSelector(".payout-end-month")));
                payoutEndMonth.SelectByValue("12");
            }
            catch
            {
            }


            try
            {
                //class="export-csv-link" => click to download
                var AExportCsvLinks = driver.FindElements(By.CssSelector("a.export-csv-link"));
                AExportCsvLinks[1].Click();

                driver.JustWait(10);
                return true;
            }
            catch
            {
                return false;
            }
            //
        }
    }
}