namespace SenStaySync.PageProcesser.AirBnb
{
    using System;
    using System.Globalization;
    using Data;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using Prices;
    using System.Threading;
    using System.Linq;
    using Tools;
    using System.Collections.Generic;
    using Exceptions;

    public static class AirBnbCalendar
    {
        public static void SetPricesForUnit(IWebDriver driver, PropertyInfo property, AirbnbUnitDailyPriceList prices)
        {
            NavigateToCalendar(driver, property);
            NavigateToMonth(driver, new AirbnbUnitDailyPrice {Year = 2017, Month = 5});

            foreach (var p in prices)
            {
                SetDayPrice(driver, p);
            }
        }


        public static void NavigateToCalendar(IWebDriver driver, PropertyInfo property)
        {
            driver.GoTo(@"https://www.airbnb.com/manage-listing/" + property.AirbnbID + @"/calendar");
            var wait = driver.CreateWaitDriver();
            wait.CssSelector("div#footer");
        }

        /// <summary>
        /// Navigate to calendar.
        /// </summary>
        /// <param name="driver">Firefox driver.</param>
        /// <param name="listingId">Airbnb property information</param>
        public static bool NavigateToCalendar(IWebDriver driver, string listingId)
        {
            bool calendarExists = true; 
            try
            {
                driver.GoTo(@"https://www.airbnb.com/manage-listing/" + listingId + @"/calendar");
                var wait = driver.CreateWaitDriver();
                wait.CssSelector("div#footer");
                return CheckCalendarExists(driver);
            }
            catch (Exception)
            {
                calendarExists = false;
            }
            return calendarExists;
        }

        /// <summary>
        /// Check whether the calendar element exists, else say that listing id is not correct.
        /// </summary>
        /// <param name="driver">Webdriver</param>
        /// <returns>true/false</returns>
        public static bool CheckCalendarExists(IWebDriver driver)
        {
            bool calendarExists = true;
            try
            {
               IWebElement calendarElement =  driver.FindElement(By.CssSelector("div.calendar-container"));
            }
            catch(NoSuchElementException)
            {
                calendarExists = false;
            }
            return calendarExists;
        }

        public static void NavigateToMonth(IWebDriver driver, AirbnbUnitDailyPrice dailyPrice)
        {
            var num = 0;

            while (num++ < 2)
            {
                try
                {
                    var div = driver.FindElement(By.CssSelector("div.calendar-month__dropdown.js-ignore-settings-hide"));
                    div.Click();
                    var el = div.FindElement(By.TagName("select"));
                    var formattedValue = CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(dailyPrice.Month) + " " + dailyPrice.Year;
                    el.Click();
                    el.SendKeys(formattedValue);
                    driver.JustWait(4);
                }
                catch
                {
                    N.Note(num + "");
                }
            }
        }

        /// <summary>
        /// Navigates to the correct month for which the listed property price should be updated in the Airbnb site.
        /// </summary>
        /// <param name="driver">Firefox driver.</param>
        /// <param name="listedMonth">Listed month for which the property price should be updated.</param>
        /// <param name="listedYear">Year to which the earlier month is part of.</param>
        public static bool NavigateToMonth(IWebDriver driver, int listedMonth, int listedYear)
        {
            bool navigateToMonth = true;
            try
            {
                var div = driver.FindElement(By.CssSelector("div.calendar-month__dropdown.js-ignore-settings-hide"));
                var selectElement = div.FindElement(By.TagName("select"));
                SelectElement select = new SelectElement(selectElement);
                string currentSelectedYearDate = select.SelectedOption.Text;
                string currentSelectedYearDateValue = select.SelectedOption.GetAttribute("value");
                string formattedText = CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(listedMonth) + " " + listedYear;

                if (!string.Equals(currentSelectedYearDate, formattedText, StringComparison.InvariantCultureIgnoreCase))
                {
                    string[] values = currentSelectedYearDateValue.Split(new char[] { '-' });
                    int year = Convert.ToInt32(values[0]);
                    int month = Convert.ToInt32(values[1]);
                    string yearMonthValue = string.Empty;

                    if ((year == listedYear && month < listedMonth) || year < listedYear)
                    {
                        // current date is smaller than listed date
                        IWebElement btnNext = driver.FindElement(By.CssSelector("button.calendar-month__nav-btns-next"));
                        while (!string.Equals(yearMonthValue, formattedText, StringComparison.InvariantCultureIgnoreCase))
                        {
                            btnNext.Click();
                            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                            wait.Until(ExpectedConditions.ElementExists(By.TagName("select")));
                            yearMonthValue = select.SelectedOption.Text;
                        }
                    }
                    else
                    {
                        IWebElement btnPrev = driver.FindElement(By.CssSelector("button.calendar-month__nav-btns-prev"));
                        while (!string.Equals(yearMonthValue, formattedText, StringComparison.InvariantCultureIgnoreCase))
                        {
                            btnPrev.Click();
                            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                            wait.Until(ExpectedConditions.ElementExists(By.TagName("select")));
                            yearMonthValue = select.SelectedOption.Text;
                        }
                    }
                }
            }
            catch(Exception)
            {
                navigateToMonth = false;
            }
            return navigateToMonth;
        }

        public static void SetDayPrice(IWebDriver driver, AirbnbUnitDailyPrice dailyPrice)
        {
            if (dailyPrice.DU() < DateTime.UtcNow.AddDays(-1)) return;

            if (!CheckMonth(driver, dailyPrice))
            {
                NavigateToMonth(driver, dailyPrice);
            }

            //tile-selection-container
            var Valid = false;
            var CurrentMonth = false;

            //@TODO: Localization support
            var MonthMark = dailyPrice.DU().ToString("MMM").ToLower();

            //var dayNumbers = Driver.FindElements(By.CssSelector("span.day-number"));
            var dateDivs = driver.FindElements(By.CssSelector("div.date"));
            foreach (var dateDiv in dateDivs)
            {
                //var spans = dayNumber.FindElements(By.TagName("span")); foreach (var span in spans) {
                if (!CurrentMonth)
                {
                    try
                    {
                        var monthSpan = dateDiv.FindElement(By.CssSelector("span.month"));
                        if (monthSpan == null) continue;
                        var spansInMonth = monthSpan.FindElements(By.TagName("span"));
                        if (spansInMonth == null) continue;
                        foreach (var spanInMonth in spansInMonth)
                        {
                            var textInMonth = (spanInMonth.Text + "").Trim().ToLower();
                            if (textInMonth == MonthMark)
                            {
                                CurrentMonth = true;
                                break;
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (CurrentMonth)
                {
                    try
                    {
                        var daySpan = dateDiv.FindElement(By.CssSelector("span.day-number"));
                        if (daySpan == null) continue;
                        var spansInDay = daySpan.FindElements(By.TagName("span"));
                        if (spansInDay == null) continue;
                        foreach (var spanInDay in spansInDay)
                        {
                            var textInDay = (spanInDay.Text + "").Trim().ToLower();
                            if (textInDay.ExtractNumber() == dailyPrice.Day && CurrentMonth)
                            {
                                Valid = true;
                                spanInDay.Click();
                                driver.JustWait(1);
                                break;
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (Valid) break;
            }

            if (!Valid) return;

            try
            {
                var input = driver.FindElementByCssSelector("input.input-giant.sidebar-price.embedded-currency__input");
                if (input == null) return;
                input.Clear();
                input.SendKeys(dailyPrice.Price + "");

                driver.FindElementByCssSelector("button.btn.btn-host[type=\"submit\"]").Click();
                driver.JustWait(1);
                N.Note(dailyPrice.Log());
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Update Airbnb unit price 
        /// </summary>
        /// <param name="driver">Firefox driver.</param>
        /// <param name="dateToBeUpdated">Date to be updated.</param>
        /// <param name="unitPrice">Airbnb unit price.</param>
        public static void UpdateUnitPrice(
                        IWebDriver driver, 
                        DateTime dateToBeUpdated,
                        double unitPrice)
        {
            int dayNumber = dateToBeUpdated.Day;
            string listingDate = dateToBeUpdated.ToString("yyyy-MM-dd");
            string monthName = dateToBeUpdated.ToString("MMM");
            IWebElement dayCell = null;

            try
            {
                driver.JustWait(10);

                if (dayNumber == 1)
                {
                    dayCell = driver.FindElement(By.XPath("//ul/li/div[@class='date']/span[contains(text(),'" + monthName + "')]/parent::div/parent::li"));
                }
                else
                {
                    dayCell = driver.FindElement(By.XPath("//ul/li/div[@class='date']/span[contains(text(),'"+ monthName +"')]/parent::div/parent::li/following-sibling::li["+(dayNumber - 1)+"]"));
                }

                if (!CheckIsDateReserved(dayCell, driver))
                {
                    
                    driver.JustWait(10);
                    //var dateVal = dayCell.FindElement(By.CssSelector("div.date"));
                    //dateVal.Click();
                    dayCell.Click();

                    driver.JustWait(10);
                    if (!CheckIsDateBlocked(dayCell, driver))
                    {
                        driver.JustWait(10);
                        var rateInput = driver.FindElement(By.CssSelector("input.embedded-currency__input"));
                        if (rateInput == null) return;
                        rateInput.Clear();
                        rateInput.SendKeys(unitPrice + "");

                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                        //     driver.FindElement(By.CssSelector("button.btn.btn-host[type=\"submit\"]")).Click();
                        driver.FindElement(By.XPath("//div[@class='calendar-edit-form panel host-calendar-sidebar-item']//button[@class='btn btn-primary' and @type='submit']")).Click();

                    }
                    else
                    {
                        throw new PriceUpdateInformation("Price for the following day was not updated as the day is set as blocked in AirBnb : " + listingDate);
                    }
                }
                else
                {
                    throw new PriceUpdateInformation("Price for the following day was not updated as it was already reserved : " + listingDate);

                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(PriceUpdateInformation))
                {
                    throw ex;
                }
                else
                {
                    throw new PriceUpdateException("Unable to Change Price", ex);
                }
            }
        }

        public static bool CheckMonth(IWebDriver driver, AirbnbUnitDailyPrice dailyPrice)
        {
            var div = driver.FindElement(By.CssSelector("div.calendar-month__dropdown.js-ignore-settings-hide"));
            var select = new SelectElement(div.FindElement(By.TagName("select")));
            return select.AllSelectedOptions[0].GetAttribute("value") == dailyPrice.MonthValue();
        }

        public static bool CheckIsDateReserved(IWebElement dayElement)
        {
            bool isReserved = true;
            
            try
            {
                isReserved = dayElement.FindElements(By.CssSelector("div.reservation-bar")).Count > 0;
                //var reservedDay = dayElement.FindElement(By.CssSelector("div.reservation-bar"));
            }
            catch (NoSuchElementException)
            {
                isReserved = false;
            }
            return isReserved;
        }

        public static bool CheckIsDateReserved(IWebElement dayElement, IWebDriver driver)
        {
            bool isReserved = true;
            //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(100));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(1));

            try
            {
                //isReserved = dayElement.FindElements(By.CssSelector("div.reservation-bar")).Count > 0;
                var reservedDay = dayElement.FindElement(By.CssSelector("div.reservation-bar"));
                //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(20000));
            }
            catch (NoSuchElementException)
            {
                ///driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(20000));
                isReserved = false;
            }
            return isReserved;
        }


       public static bool CheckIsDateBlocked(IWebElement dayElement, IWebDriver driver)
        {
            IWebElement blockedDay = null;
            bool isBlocked = false;
            try
            {
                IWebElement radioButtonElement = driver.FindElement(By.XPath("//div[@class='host-calendar-sidebar']//div[@class='panel-body']//div[@class='space-2 space-top-6']//span//fieldset//div[@class='space-2 row category-radio-option'][1]//div[@class='col-1']//input[@id='availability_selection_available']"));
                ///var selectedSpan = driver.FindElement(By.XPath("//div[@class='segmented-control segmented-control--block segmented-control--large']//label[@class='segmented-control__option segmented-control__option--selected']//span"));

                if (!radioButtonElement.Selected)
                {
                    isBlocked = true;
                }
                //if(selectedSpan.Text.ToLower() == "blocked")
                //{
                //    isBlocked = true;
                //}
            }
            catch(Exception)
            {
                blockedDay = dayElement.FindElementSafe(By.CssSelector("div.unavailable-rule-bar"));
            }
            finally
            {
                if(!isBlocked && blockedDay != null)
                {
                    isBlocked = true;
                }
            }
            return isBlocked;
        }


        public static bool CheckIsTitleDisplayed(IWebElement dayElement, IWebDriver driver)
        {
            //title- notes
            bool titleNotesExists = false;
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(1));

            try
            {
                var titleNotes = dayElement.FindElement(By.CssSelector("div.tile-notes"));
                if(titleNotes != null)
                {
                    titleNotesExists = true;
                }
                
            }
            catch (Exception)
            {
                titleNotesExists = false;
            }
            return titleNotesExists;
        }

        public static void ZoomOut(IWebDriver driver)
        {
            //To zoom out page 4 time using CTRL and - keys.
            for (int i = 0; i < 4; i++)
            {
                driver.FindElement(By.TagName("html")).SendKeys(Keys.Control + Keys.Subtract);
            }
        }

    }
}