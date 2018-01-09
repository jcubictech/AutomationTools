using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SenStaySync.Data;
using SenStaySync.Test;
using SenStaySync.Prices;
using System.Threading;
using SenStaySync.Tools;

namespace SenStaySync.PageProcesser.Streamline
{
    public static class StreamlineDailyPrice
    {

        public static void SetPricesForUnit(IWebDriver driver, int UnitEditId, List<SeasonPrice> Prices, bool IsTest = false)
        {
            NavigateToPage(driver, UnitEditId);
            CloseControlPanel(driver);
            NavigateToDailyPriceTab(driver);
            SetPrices(driver, Prices);
            Submit(driver);
        }

        public static void SetPricesForUnit(IWebDriver driver, int UnitEditId, List<Price> Prices, OperationsJsonLogger<PricePushResult> pricePushLog = null)
        {
            try
            {
                NavigateToPage(driver, UnitEditId);
                CloseControlPanel(driver);
                if (NavigatToDailyPricingTab(UnitEditId, driver, pricePushLog))
                {
                    SetPrices(driver, Prices, UnitEditId, pricePushLog);
                    if(Submit(driver))
                    {
                        pricePushLog.Log(new PricePushResult(Channel.StreamLine, PricePushLogArea.PriceUpdate, PricingPushLogType.Information, "Price updated successfully for the property: "+UnitEditId));
                    }
                }
            }
            catch(Exception ex)
            {
                pricePushLog.Log(new PricePushResult(Channel.StreamLine, PricePushLogArea.PriceUpdate, PricingPushLogType.Error, ex.Message, Config.I.StreamLineAccountLogin, UnitEditId.ToString(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
            }
        }

        private static void CloseControlPanel(IWebDriver driver)
        {
            try
            {
                var aClose = driver.FindElementByCssSelector("a[class=container-close]");
                aClose.Click();
            }
            catch
            {

            }
        }

        public static void NavigateToPage(IWebDriver driver, int UnitEditId)
        {
            var url = Config.I.StreamLineEditHomeURL + UnitEditId;
            driver.GoTo(url);
            var wait = driver.CreateWaitDriver();

            wait.CssSelector(".yui-content");
        }

        private static void NavigateToDailyPriceTab(IWebDriver driver)
        {
            var aList = driver.FindElements(By.CssSelector(".yui-nav a"));
            foreach (var a in aList)
            {
                var em = a.FindElement(By.TagName("em"));
                if (em == null) continue;
                if ((em.Text + "").Trim().ToLower() == "daily pricing")
                {
                    a.Click();
                    return;
                }
            }
            throw new Exception("Nav not loaded correctly or was changed");
        }

        private static bool NavigatToDailyPricingTab(int homeId, IWebDriver driver, OperationsJsonLogger<PricePushResult> pricePushLogger)
        {
            //div[@id='home_tabs']/ul[@class='yui-nav']//li/a/descendant::em[text()='Daily Pricing']
            bool navigateToPricingTab = true;
            try
            {
                var dailyPricingLink = driver.FindElement(By.XPath("//div[@id='home_tabs']/ul[@class='yui-nav']//li/a/descendant::em[text()='Daily Pricing']"));
                dailyPricingLink.Click();
            }
            catch (NoSuchElementException)
            {
                navigateToPricingTab = false;
                pricePushLogger.Log(new PricePushResult(Channel.StreamLine, PricePushLogArea.PriceUpdate, PricingPushLogType.Error, "Cannot navigate to daily pricing tab :"+homeId.ToString()));
            }
            return navigateToPricingTab;

        }

        private static void SetPrices(IWebDriver driver, List<SeasonPrice> Prices)
        {
            
            foreach (var p in Prices)
            {
                var nameEndsWith = "_" + p.SeasonId + "_" + "price1";
                var inputs = driver.FindElements(By.CssSelector("input[name$=\""+nameEndsWith+"\"]"));
                foreach (var i in inputs)
                {
                    //try {
                    var name = i.GetAttribute("name");
                    var v = i.GetAttribute("readonly");
                    if (v == "true") continue;
                    
                    //Console.WriteLine("r="+v + " n=" + name);
                    i.Clear();
                    i.SendKeys(p.Price + "");
                        //driver.ExecuteJS("document.getElementById('"+ name+"').value = \"" + p.Price + "\";");
                    //} catch { }

                }
                //p.SeasonId
            }
            //value_1415_57661_13714566_price1
            //value_1489_57661_13714566_price1

            //value_1415_57661_13714568_price1
            //value_1415_57661_13714565_price1
        }

        /// <summary>
        /// Set the property's daily price for the various seasons.
        /// </summary>
        /// <param name="driver">Web driver.</param>
        /// <param name="dailyPrices">Daily prices.</param>
        private static void SetPrices(IWebDriver driver, List<Price> dailyPrices, int homeId, OperationsJsonLogger<PricePushResult> pricePushLogger)
        {
            foreach(Price price in dailyPrices)
            {
                try
                {
                    // The delay here is needed so that the screen doesnt refresh like a bullet.
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                    if (string.IsNullOrEmpty(price.SeasonId) || price.SeasonId == "0") continue;

                    string nameStartsWith = "value_1415";
                    string nameEndsWith = price.SeasonId + "_" + "price1";

                    var priceInput = driver.FindElement(By.XPath("//input[starts-with(@name,'" + nameStartsWith + "') and contains(@name,'" + nameEndsWith + "')]"));

                    var readOnly = priceInput.GetAttribute("readonly");
                    if (readOnly == "true") continue;

                    priceInput.Clear();
                    priceInput.SendKeys(price.PropertyPrice.ToString());
                }
                catch(Exception ex)
                {
                    pricePushLogger.Log(new PricePushResult(Channel.StreamLine,
                                                            PricePushLogArea.PriceUpdate,
                                                            PricingPushLogType.Error,
                                                            ex.Message,
                                                            Config.I.StreamLineAccountLogin,
                                                            homeId.ToString(),
                                                            price.SeasonStartDate.ToString("MMM dd") + " - " + price.SeasonEndDate.ToString("MMM dd") + " " + price.SeasonEndDate.Year.ToString(),
                                                            string.Empty,   // Property Name
                                                            string.Empty,   // Property Code
                                                            price.PropertyPrice.ToString(),   // Property Price
                                                            ex.Message,  // Exception Message
                                                            string.Empty  // Proxy IP

                                                            ));
                }
            }
        }

        private static bool Submit(IWebDriver driver)
        {
            var submitBtn = driver.FindElementByCssSelector("input[name=submit_pricing_button]");
            submitBtn.Click();
            //driver.JustWait(2);
            return CheckWhetherPricesUpdated(driver);
        }

        /// <summary>
        /// Checks whether the prices has been updated correctly or not.
        /// </summary>
        /// <param name="driver">Web driver</param>
        /// <returns>true/false</returns>
        private static bool CheckWhetherPricesUpdated(IWebDriver driver)
        {
            bool pricesUpdatedSuccessfully = false;
            try
            {
                var priceUpdateWait = driver.CreateWaitDriver(10);
                priceUpdateWait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@id='main_message_div']//td[@id='main_message_td']")));

                IWebElement successMessage = driver.FindElement(By.XPath("//div[@id='main_message_div']//td[@id='main_message_td']"));

                if (successMessage.Text.ToLower().Contains("pricing has been updated"))
                {
                    pricesUpdatedSuccessfully = true;
                }

            }
            catch (Exception)
            {
                pricesUpdatedSuccessfully = false;
            }
            return pricesUpdatedSuccessfully;

        }


        public static void RunPriceScript(IWebDriver driver, PriceScript script)
        {
            var prices = new List<SeasonPrice>();
            foreach (var Command in script.Commands)
            {
                if (Command.Type == PriceScriptCommand.TYPE_SEASON_GROUP)
                {
                    N.Note(string.Format("Changing season group for {0} to {1}", script.UnitId, Command.SeasonGroupID));
                    StreamlineSeasonProcesser.ChangeSeasonGroup(driver, Command.StreamlineEditId + "", Command.SeasonGroupID);
                }
                else if (Command.Type == PriceScriptCommand.TYPE_PRICE)
                {
                    prices.Add(Command);
                }
            }

            if (prices.Count > 0)
            {
                N.Note(string.Format("Set daily prices for {0}, count of updates: {1}", script.UnitId, prices.Count));
                SetPricesForUnit(driver, script.StreamlineEditId, prices);
            }

        }

        /// <summary>
        /// Check whether property exists or not.
        /// </summary>
        /// <param name="driver">Web driver.</param>
        /// <param name="homeId">Home id.</param>
        /// <returns>true/false</returns>
        public static bool CheckWhetherPropertyExists(IWebDriver driver, string homeId)
        {
            bool propertyExists = true;
            try
            { 
                NavigateToPage(driver, Convert.ToInt32(homeId));
            }
            catch(Exception)
            {
                propertyExists = false;
            }
            return propertyExists;
        }

    }
}