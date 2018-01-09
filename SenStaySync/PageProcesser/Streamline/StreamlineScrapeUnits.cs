namespace SenStaySync.PageProcesser.Streamline
{
    using System;
    using System.Collections.ObjectModel;
    using Data;
    using Data.Streamline;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using Scrapers;

    public class StreamlineScrapeUnits
    {
        public static void Process(IWebDriver driver, bool grabAdditionalInfo)
        {
            var URL = @"https://www.resortpro.net/new/admin/index.html";

            driver.GoTo(URL);
            var wait = driver.CreateWaitDriver();

            //<div id="dock-container"  
            var dockContainerElement = driver.FindElement(By.Id("dock-container"));
            //<a href="javascript:YAHOO.frontdesk.UpdateContentCached('menu_configuration.html', 'dedicated=1')"
            var configurationLinkElement = dockContainerElement.FindAByHrefStartsWith("javascript:YAHOO.frontdesk.UpdateContentCached('menu_configuration.html',");

            configurationLinkElement.Click();
            driver.JustWait(1);

            //<div id="menuExpandedContainer">
            var menuExpandedContainerElement = driver.FindElement(By.Id("menuExpandedContainer"));

            //<a href="javascript:YAHOO.frontdesk.UpdateContent('general_homes.html', 'lodging_type_id=3&amp;status_id=2');">Vacation Rentals (170)</a>
            var vacationRentalsElement = menuExpandedContainerElement.FindAByHrefStartsWith("javascript:YAHOO.frontdesk.UpdateContent('general_homes.html',");
            vacationRentalsElement.Click();
            driver.JustWait(5);

            //<select id="status_id" style="width:100px;" size="1" name="status_id">

            // set type to AirBnb
            var statusIdElement = new SelectElement(driver.FindElement(By.Id("status_id")));
            // 80 is AirBnb's magic number
            //<option value="0">Any Status</option>
            statusIdElement.SelectByValue("0");
            driver.JustWait(1);


            //<form id="table_navigation_form" action="javascript:YAHOO.frontdesk.table_navigation_function();" method="get">
            var navFormElement = driver.FindElement(By.Id("table_navigation_form"));

            //<input type="submit" value="GO" name="ss"> // Can be passed
            var goButtonElement = navFormElement.FindElement(By.CssSelector("[value=\"GO\"]"));

            goButtonElement.Click();
            driver.JustWait(4);

            //<input type="button" onclick="javascript:table_pager_submit('table_navigation_form', 1, 1, '');" value="Show All" name="button_view_all">
            var showAllInputElement = driver.FindElement(By.CssSelector("[name=\"button_view_all\"]"));
            //var showAllInputElement = navFormElement.FindTagByAttributeStartsWith("input", "onclick", "javascript:table_pager_submit('table_navigation_form',");

            showAllInputElement.Click();

            driver.JustWait(10);
            driver.FindElement(By.Id("frontdesk_content"));

            var basetableElement = driver.FindElement(By.CssSelector(".basetable.fixedHeader"));

            var trs = basetableElement.FindElements(By.TagName("tr"));

            var i = 0;
            var items = new Collection<StreamlinePropertyInfo>();
            foreach (var tr in trs)
            {
                //if (i++ == 0) continue;

                var propertyItem = new StreamlinePropertyInfo();

                var editLinkElement = tr.FindTagByAttributeStartsWith("a", "onclick", "javascript:HomeEdit(");
                if (editLinkElement == null)
                {
                    //Console.WriteLine("ERROR");
                    continue;
                }
                i++;

                var hrefAttr = editLinkElement.GetAttribute("onclick");

                var streamlineEditId = hrefAttr.ExtractNumber();
                propertyItem.StreamlineEditID = streamlineEditId;

                items.Add(propertyItem);

                Console.WriteLine("Property #" + i);
            }


            var updated = new StreamlinePropertyCollection();
            i = 0;

            foreach (var item in items)
            {
                if (item.StreamlineEditID <= 0) continue;
                i++;

                var numTrys = 0;
                loopMark:

                try
                {
                    var propertyScraper = new PropertyScraper(driver);
                    propertyScraper.Scrap(item, grabAdditionalInfo);
                    updated.Add(item);
                }
                catch
                {
                    if (numTrys++ < 3)
                    {
                        Console.WriteLine("Retry #" + numTrys + " " + item.StreamlineEditID + " " + i);
                        goto loopMark;
                    }
                }
            }

            updated.Save();
            Console.Write(Config.I.PropertyCollectionJsonPath + " saved at " + updated.DateCreated);

            var Json = updated.ToJson();
            Console.Write(Json);
        }

        /// <summary>
        /// Switching to popup, and returns current windows handler
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="elementToClickOn"></param>
        /// <returns></returns>
        private static void Popup(IWebDriver driver, IWebElement elementToClickOn)
        {
            var finder = new PopupWindowFinder(driver, TimeSpan.FromSeconds(20));
            var popupWindowHandle = finder.Click(elementToClickOn);

            driver.SwitchTo().Window(popupWindowHandle);
        }
    }
}