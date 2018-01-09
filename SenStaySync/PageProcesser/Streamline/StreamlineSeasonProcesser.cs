namespace SenStaySync.PageProcesser.Streamline
{
    using System;
    using Data;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using System.Threading;
    using System.Globalization;

    public static class StreamlineSeasonProcesser
    {
        public static void CreateSeason(IWebDriver driver, int Year, int Month, int Day, string groupId)
        {
            var date = new DateTime(Year, Month, Day);

            var url = @"https://www.resortpro.net/new/admin/edit_company_season.html?id=0&lodging_type_id=3";

            driver.GoTo(url);

            var _wait = new WebDriverWait(driver, new TimeSpan(0, 0, Config.I.RegularWaitSeconds*2));

            // ждем пока загрузится
            //_wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".transaction-table")));
            //_wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[name=close_button]")));

            // Wait for existing of close button 
            _wait.CssSelector("[name=close_button]");


            // Group 
            var group_id = driver.FindSelectElementById("group_id");
            //20250 - 366 debug
            //20096 - 365 days
            group_id.SelectByValue(groupId);
            driver.JustWait(1);


            // Name
            var nameElement = driver.FindElementById("name");
            var SeasonName = date.ToString("MM-dd");
            nameElement.SendKeys(SeasonName);


            var dailyCheckboxElement = driver.FindElementByCssSelector("input[type=\"checkbox\"][value=\"1\"]");
            dailyCheckboxElement.Click();
            //Use Daily Pricing       <input type="checkbox" value="1" name="use_pricing_model[]"></td></tr>


            // Description
            var descriptionTabA = driver.FindElementByCssSelector("a[href=\"#tab2\"]");
            descriptionTabA.Click();
            var descriptionElement = driver.FindElementByCssSelector("textarea[name=\"description\"]");
            descriptionElement.SendKeys(date.ToString("MMMM dd"));


            CreatePeriod(driver, Year, Month, Day);
            // Periods
            /*            for (var Year = 2016; Year < 2026; Year++)
                        {
                            CreatePeriod(driver, Year, Month, Day);
                        }
              //*/
        }

       
        /// <summary>
        /// Create seasons for the season group passed.
        /// </summary>
        /// <param name="driver">Web driver.</param>
        /// <param name="seasonGroupId">Season group id.</param>
        /// <param name="seasonStartDate">Season start date.</param>
        /// <param name="seasonEndDate">Season end date.</param>
        public static string CreateSeason(IWebDriver driver, string seasonGroupId, DateTime seasonStartDate, DateTime seasonEndDate, int minNightsStay = 0)
        {
            string editSeasonURL = Config.I.StreamLineCreateSeasonURL;
            string seasonId = string.Empty;

            try
            {
                driver.GoTo(editSeasonURL);
                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, Config.I.RegularWaitSeconds * 2));
                wait.CssSelector("[name=close_button]");

                // Group drop down. 
                IWebElement groupDrpDwnLstBox = driver.FindElement(By.Id("group_id"));
                // This code should be reviewed and changed when the next version of Javascript releases.
                ((IJavaScriptExecutor)driver).ExecuteScript("var select = arguments[0]; for(var i = 0; i < select.options.length; i++){ if(select.options[i].value == arguments[1]){ select.options[i].selected = true; } }", groupDrpDwnLstBox, seasonGroupId);

                // Setting the name of the season.
                var nameElement = driver.FindElementById("name");
                nameElement.Clear();
                string seasonName = GetSeasonName(seasonStartDate, seasonEndDate);
                nameElement.SendKeys(seasonName);

                // Setting the minimum nights stay 
                IWebElement minNightsDrpDwnLstBox = driver.FindElement(By.Id("narrow_defined_days"));
                ((IJavaScriptExecutor)driver).ExecuteScript("var select = arguments[0]; for(var i = 0; i < select.options.length; i++){ if(select.options[i].value == arguments[1]){ select.options[i].selected = true; } }", minNightsDrpDwnLstBox, minNightsStay);


                // Use daily pricing checkbox.
                var dailyCheckboxElement = driver.FindElementByCssSelector("input[type=\"checkbox\"][value=\"1\"]");
                dailyCheckboxElement.Click();

                // As of now no description needs to be set.

                // Set the period for the seasons.
                CreatePeriod(driver, seasonStartDate, seasonEndDate, seasonName);

                // Check whether season has been created successfully
                CheckWhetherSeasonCreated(driver);

                driver.JustWait(10);
                // Get the season id.
                seasonId = GetId("id", driver);

            }
            catch(Exception)
            {
                seasonId = string.Empty;
            }

            return seasonId;

        }

        private static void CreatePeriod(IWebDriver driver, int Year, int Month, int Day)
        {
            if (Month == 2 && Day == 29 && !DateTime.IsLeapYear(Year))
            {
                return;
            }

            var date = new DateTime(Year, Month, Day);
            var PeriodsTabA = driver.FindElementByCssSelector("a[href=\"#tab4\"]");
            PeriodsTabA.Click();

            driver.FindElementById("new_name").SendKeys(date.ToString("yyyy MMMM d"));

            var dateStr = date.ToString("MM/dd/yyyy");

            driver.ExecuteJS("document.getElementById('date_start_new').value = \"" + dateStr + "\";");
            driver.ExecuteJS("document.getElementById('date_end_new').value = \"" + dateStr + "\";");

            driver.FindElementByCssSelector("[name=submit]").Click();
            driver.JustWait(1);
        }

        /// <summary>
        /// Create period for the season
        /// </summary>
        /// <param name="driver">Web driver.</param>
        /// <param name="seasonStartDate">Season start date.</param>
        /// <param name="seasonEndDate">Season end date.</param>
        private static void CreatePeriod(IWebDriver driver, DateTime seasonStartDate, DateTime seasonEndDate, string seasonName)
        {
            string dateTimeFormat = "MM/dd/yyyy";
            var periodTab = driver.FindElementByCssSelector("a[href=\"#tab4\"]");
            periodTab.Click();

            driver.FindElementById("new_name").SendKeys(seasonName);

            var dateStartNew = seasonStartDate.ToString(dateTimeFormat, CultureInfo.InvariantCulture);
            var dateEndNew = seasonEndDate.ToString(dateTimeFormat, CultureInfo.InvariantCulture);

            driver.ExecuteJS("document.getElementById('date_start_new').value = \"" + dateStartNew + "\";");
            driver.ExecuteJS("document.getElementById('date_end_new').value = \"" + dateEndNew + "\";");

            driver.FindElementByCssSelector("[name=submit]").Click();
            driver.JustWait(1);
        }

        public static void ScrapeSeasons(IWebDriver driver, string groupId)
        {
            var URL = @"https://www.resortpro.net/new/admin/index.html";
            driver.GoTo(URL);

            var wait = driver.CreateWaitDriver();

            //<div id="dock-container"  
            var dockContainerElement = driver.FindElement(By.Id("dock-container"));

            //<a href="javascript:YAHOO.frontdesk.UpdateContentCached('menu_configuration.html', 'dedicated=1')"
            var configurationLinkElement =
                dockContainerElement.FindAByHrefStartsWith(
                    "javascript:YAHOO.frontdesk.UpdateContentCached('menu_configuration.html',");
            configurationLinkElement.Click();

            driver.JustWait(1);

            //<div id="menuExpandedContainer">
            var menuExpandedContainerElement = driver.FindElement(By.Id("menuExpandedContainer"));

            //<a href="javascript:YAHOO.frontdesk.UpdateContent('general_homes.html', 'lodging_type_id=3&amp;status_id=2');">Vacation Rentals (170)</a>
            var vacationRentalsElement =
                menuExpandedContainerElement.FindAByHrefStartsWith(
                    "javascript:YAHOO.frontdesk.UpdateContent('general_homes.html',");
            vacationRentalsElement.Click();

            driver.JustWait(1);

            var seasonsButton = driver.FindElementByCssSelector("[value=\"SEASONS\"]");
            seasonsButton.Click();
            wait.CssSelector("[name=\"pages_groups_button\"]");
            driver.JustWait(1);

            var groupsButton = driver.FindElementByCssSelector("[name=\"pages_groups_button\"]");
            groupsButton.Click();
            wait.CssSelector("[value=\"Add Season Group\"]");
            driver.JustWait(1);
            //name="pages_groups_button"


            var Sucess = false;
            var seasonGroupAList =
                driver.FindElements(
                    By.CssSelector(
                        "a[onclick^=\"javascript:YAHOO.frontdesk.UpdateContent('general_companies_seasons.html',\"]"));
            foreach (var seasonGroupA in seasonGroupAList)
            {
                var onClickAttr = seasonGroupA.GetAttribute("onclick");
                if (onClickAttr.Contains(groupId))
                {
                    seasonGroupA.Click();
                    Sucess = true;
                    break;
                }
            }
            if (!Sucess) return;

            driver.JustWait(10);
            wait.CssSelector("[name=\"button_view_all\"]");
            driver.JustWait(1);

            var ViewAllButton = driver.FindElementByCssSelector("[name=\"button_view_all\"]");
            ViewAllButton.Click();
            wait.CssSelector("[name=\"pages_groups_button\"]");

            driver.JustWait(10);

            var basetableElement = driver.FindElement(By.CssSelector(".basetable.fixedHeader"));
            var trs = basetableElement.FindElements(By.TagName("tr"));

            var i = 0;
            var SeasonGroup = new StreamlineSeasonGroup();
            SeasonGroup.ID = groupId.ExtractNumber();

            foreach (var tr in trs)
            {
                //var editLinkElement = tr.FindElement(By.CssSelector("a[onclick^=\"javascript:EditCompanySeason(\"]"));
                var editLinkElement = tr.FindTagByAttributeStartsWith("a", "onclick", "javascript:EditCompanySeason(");
                if (editLinkElement == null)
                {
                    //Console.WriteLine("ERROR");
                    continue;
                }
                i++;

                var tds = tr.FindElements(By.CssSelector("td[align=\"left\"]"));
                var nameTd = tds[0];

                var hrefAttr = editLinkElement.GetAttribute("onclick");

                var SeasonID = hrefAttr.ExtractNumber();
                var SeasonName = nameTd.Text;

                SeasonGroup.Add(SeasonID, SeasonName);

                Console.WriteLine("Season " + SeasonName + " processed");

                //if (i > 5) break;
            }

            SeasonGroup.Save();
        }

        public static void ChangeSeasonGroup(IWebDriver driver, string StreamlineEditId, int SeasonGroupID,
           bool IsTest = false)
        {
            driver.Navigate().GoToUrl(@"https://www.resortpro.net/new/admin/edit_home.html?home_id=" + StreamlineEditId);
            var wait = driver.CreateWaitDriver(30);
            wait.CssSelector("[id=season_group_id]");
            
            IWebElement selectGroupElement = driver.FindElement(By.Id("season_group_id"));
            ((IJavaScriptExecutor)driver).ExecuteScript("var select = arguments[0]; for(var i = 0; i < select.options.length; i++){ if(select.options[i].value == arguments[1]){ select.options[i].selected = true; } }", selectGroupElement, SeasonGroupID);

            driver.JustWait(10);

            var submit = driver.FindElementByCssSelector("[name=modify_button]");
            submit.Click();

            var waitDriver = driver.CreateWaitDriver(5);
            waitDriver.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector("[id=yui-gen0-button]")));

            var confirm = driver.FindElementByCssSelector("[id=yui-gen0-button]");
            confirm.Click();
            //driver.JustWait(20);
            //driver.Navigate().GoToUrl(@"https://www.resortpro.net/new/admin");
        }

        /// <summary>
        /// Change season group.
        /// </summary>
        /// <param name="driver">Web driver</param>
        /// <param name="streamLineEditId">Streamline home id.</param>
        /// <param name="streamLineHomeName">Streamline home name.</param>
        /// <param name="seasonGroupId">Streamline group id.</param>
        public static bool ChangeSeasonGroup(IWebDriver driver, string streamLineEditId, string streamLineHomeName, string seasonGroupId)
        {
            driver.Navigate().GoToUrl(Config.I.StreamLineEditHomeURL + streamLineEditId);
            var wait = driver.CreateWaitDriver(30);
            wait.CssSelector("[id=season_group_id]");

            IWebElement selectGroupElement = driver.FindElement(By.Id("season_group_id"));
            ((IJavaScriptExecutor)driver).ExecuteScript("var select = arguments[0]; for(var i = 0; i < select.options.length; i++){ if(select.options[i].value == arguments[1]){ select.options[i].selected = true; } }", selectGroupElement, seasonGroupId);

            driver.JustWait(10);

            var submit = driver.FindElementByCssSelector("[name=modify_button]");
            submit.Click();

            var waitDriver = driver.CreateWaitDriver(5);
            waitDriver.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector("[id=yui-gen0-button]")));

            var confirm = driver.FindElementByCssSelector("[id=yui-gen0-button]");
            confirm.Click();

            return CheckWhetherSeasonGroupHasBeenChanged(driver, streamLineHomeName);
       }


        /// <summary>
        /// Create a new season group.
        /// </summary>
        /// <param name="driver">Firefox  web driver</param>
        /// <returns>Season group id of the created group.</returns>
        public static string CreateSeasonGroup(IWebDriver driver, DateTime processStartTime)
        {
            string seasonGroupURL = Config.I.StreamLineCreateSeasonGroupURL;
            string seasonGroupId = string.Empty;

            try
            {
                driver.GoTo(seasonGroupURL);

                WebDriverWait waitForSeasonGroupInput = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                waitForSeasonGroupInput.Until(ExpectedConditions.ElementExists(By.Id("name")));

                IWebElement seasonGroupName = driver.FindElement(By.Id("name"));
                IWebElement seasonGroupSubmit = driver.FindElement(By.Name("submit"));
                string formattedSeasonGroupName = GetSeasonGroupName(processStartTime);


                seasonGroupName.SendKeys(formattedSeasonGroupName);
                seasonGroupSubmit.Click();

                if (CheckWhetherSeasonGroupCreated(driver))
                {
                    seasonGroupId = GetId("id", driver);
                }
            }
            catch(Exception)
            {
                seasonGroupId = string.Empty;
            }

            return seasonGroupId;
        }

        private static string GetId(string fieldName, IWebDriver driver)
        {
            IWebElement idElement;
            WebDriverWait waitforId = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            waitforId.Until(ExpectedConditions.ElementExists(By.Name(fieldName)));

            idElement = driver.FindElement(By.Name(fieldName));

            // Get the group id value, This value will be needed when we update the season.
            return idElement.GetAttribute("value");
        }

        /// <summary>
        /// Get season name 
        /// Jun 25 - Jun 26 2016
        /// </summary>
        /// <param name="seasonFromDate">Season from date.</param>
        /// <param name="seasonToDate">Season to date.</param>
        /// <returns>Season name.</returns>
        public static string GetSeasonName(DateTime seasonFromDate, DateTime seasonToDate)
        {
            string seasonFormat = string.Empty;
            // Jun 25 - Jun 26 2016
            if (seasonFromDate.Year == seasonToDate.Year)
            {
                seasonFormat = seasonFromDate.ToString("MMM dd") + " - " + seasonToDate.ToString("MMM dd") + " " + seasonToDate.Year.ToString();
            }
            else
            {
                seasonFormat = seasonFromDate.ToString("MMM dd") + " - " + seasonToDate.ToString("MMM dd") + " " + seasonFromDate.Year.ToString() + "/" + seasonToDate.ToString("yy");
            }

            return seasonFormat;
        }

        /// <summary>
        /// Returns the season group name
        /// </summary>
        /// <returns></returns>
        public static string GetSeasonGroupName(DateTime processStartTime)
        {
            return processStartTime.ToString("yyyy-MM-dd HH:mm") + " Season Group";
        }

        /// <summary>
        /// Check whether the season created successfully message has been shown.
        /// </summary>
        /// <param name="driver">Web driver.</param>
        /// <returns>True/false</returns>
        public static bool CheckWhetherSeasonGroupCreated(IWebDriver driver)
        {
            bool groupCreatedSuccessfully = false;
            try
            {
                IWebElement successMessage = driver.FindElement(By.XPath("//form//table//td[@class='tooltip']"));

                if (successMessage.Text.Equals("The record has been added."))
                {
                    groupCreatedSuccessfully = true;
                }
                else if (successMessage.Text.Equals("Sorry, but record already exists."))
                {
                    // Log should be written from here, which says that the record already exists.
                    groupCreatedSuccessfully = false;
                }
            }
            catch (NoSuchElementException)
            {
                // Logs can be created here.
                groupCreatedSuccessfully = false;
            }
            return groupCreatedSuccessfully;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static bool CheckWhetherSeasonCreated(IWebDriver driver)
        {
            bool seasonCreatedSuccessfully = false;
            try
            {
                var successMessageWait = driver.CreateWaitDriver(10);
                successMessageWait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//form//table//td[@class='tooltip']")));

                IWebElement successMessage = driver.FindElement(By.XPath("//form//table//td[@class='tooltip']"));

                if (successMessage.Text.Contains("The Season record has been added"))
                {
                    seasonCreatedSuccessfully = true;
                }

            }
            catch(Exception)
            {
                seasonCreatedSuccessfully = false;
            }
            return seasonCreatedSuccessfully;
        }

        private static bool CheckWhetherSeasonGroupHasBeenChanged(IWebDriver driver, string streamLineHomeName)
        {
            bool seasonGroupChangedSuccessfully = false;
            string expectedSuccessMessage = string.Format("Details have been modified", streamLineHomeName);
            //string expectedSuccessMessage = string.Format("{0} Details have been modified", streamLineHomeName);

            try
            {
                var successMessageWait = driver.CreateWaitDriver(10);
                successMessageWait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//table[@class='basetable']//td[@class='tooltip' and @id='main_message_td']")));

                IWebElement successMessage = driver.FindElement(By.XPath("//table[@class='basetable']//td[@class='tooltip' and @id='main_message_td']"));

                if (successMessage.Text.ToLower().Contains(expectedSuccessMessage.ToLower()))
                {
                    seasonGroupChangedSuccessfully = true;
                }
            }
            catch(Exception)
            {
                seasonGroupChangedSuccessfully = false;
            }
            return seasonGroupChangedSuccessfully;
        }
    }
}