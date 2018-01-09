namespace SenStaySync
{
    using System;
    using Data.Streamline;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;

    public static class SeleniumExtensions
    {
        public static object GetChildFieldValue(this IWebElement webElement, FormFieldType fieldType)
        {
            object result;
            switch (fieldType)
            {
                case FormFieldType.Input:
                    var input = webElement.FindElement(By.CssSelector("input"));
                    result = input.GetFieldValue(fieldType);
                    break;
                case FormFieldType.TextArea:
                    var textarea = webElement.FindElement(By.CssSelector("textarea"));
                    result = textarea.GetFieldValue(fieldType);
                    break;
                case FormFieldType.Select:
                    var select = webElement.FindElement(By.CssSelector("select"));
                    result = select.GetFieldValue(fieldType);
                    break;
                case FormFieldType.Checkbox:
                    var checkbox = webElement.FindElement(By.CssSelector("input"));
                    result = checkbox.GetFieldValue(fieldType);
                    break;
                default:
                    result = webElement.GetFieldValue(fieldType);
                    break;
            }
            return result;
        }

        public static object GetFieldValue(this IWebElement webElement, FormFieldType fieldType)
        {
            object result;
            switch (fieldType)
            {
                case FormFieldType.Input:
                    result = webElement.GetAttribute("value");
                    break;
                case FormFieldType.TextArea:
                    result = webElement.Text;
                    break;
                case FormFieldType.Select:
                    result = new SelectElement(webElement).SelectedOption.Text;
                    break;
                case FormFieldType.Checkbox:
                    result = webElement.Selected;
                    break;
                default:
                    result = webElement.Text;
                    break;
            }
            return result;
        }

        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds = -1)
        {
            if (timeoutInSeconds < 0) timeoutInSeconds = Config.I.RegularWaitSeconds;

            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }

        public static void GoTo(this IWebDriver driver, string URL)
        {
            driver.Navigate().GoToUrl(URL);
            driver.JustWait(5);
            //var wait = driver.CreateWaitDriver();
            //wait.Until()
        }

        public static WebDriverWait CreateWaitDriver(this IWebDriver driver, int timeoutInSeconds = -1)
        {
            if (timeoutInSeconds < 0) timeoutInSeconds = Config.I.RegularWaitSeconds;
            return new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        }

        public static void JustWait(this IWebDriver driver, int SecondsToWait = 10)
        {
            try
            {
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(SecondsToWait));
            }
            catch
            {
            }
        }


        public static IWebElement FindAByHrefStartsWith(this IWebElement element, string hrefStartsWith)
        {
            return element == null ? null : element.FindTagByAttributeStartsWith("a", "href", hrefStartsWith);
        }

        public static IWebElement FindTagByAttribute(this IWebElement element, string tagName, string attributeName, string attributeValue)
        {
            var tagList = element.FindElements(By.TagName(tagName));
            foreach (var item in tagList)
            {
                try
                {
                    var attr = item.GetAttribute(attributeName);
                    if (attr == attributeValue)
                    {
                        return item;
                    }
                }
                catch
                {
                    // ignored
                }
            }
            return null;
        }

        public static IWebElement FindTagByAttributeStartsWith(this IWebElement element, string tagName, string attributeName, string attributeValueStartsWith)
        {
            var tagList = element.FindElements(By.TagName(tagName));
            foreach (var item in tagList)
            {
                try
                {
                    var attr = item.GetAttribute(attributeName);
                    if (attr.StartsWith(attributeValueStartsWith))
                    {
                        return item;
                    }
                }
                catch
                {
                    // ignored
                }
            }
            return null;
        }

        public static object ExecuteJS(this IWebDriver driver, string jsToRun)
        {
            try
            {
                return ((IJavaScriptExecutor) driver).ExecuteScript(jsToRun);
            }
            catch
            {
                return null;
            }
        }

        public static object CssSelector(this WebDriverWait waitObj, string cssSelector)
        {
            return waitObj.Until(ExpectedConditions.ElementExists(By.CssSelector(cssSelector)));
        }

        public static IWebElement FindElementById(this IWebDriver driver, string ID)
        {
            return driver.FindElement(By.Id(ID));
        }

        public static IWebElement FindElementByCssSelector(this IWebDriver driver, string CssSelector)
        {
            return driver.FindElement(By.CssSelector(CssSelector));
        }

        public static SelectElement FindSelectElement(this IWebDriver driver, By By)
        {
            return new SelectElement(driver.FindElement(By));
        }

        public static SelectElement FindSelectElementById(this IWebDriver driver, string ID)
        {
            return new SelectElement(driver.FindElement(By.Id(ID)));
        }

        public static SelectElement FindSelectElementByCssSelector(this IWebDriver driver, string CssSelector)
        {
            return new SelectElement(driver.FindElement(By.CssSelector(CssSelector)));
        }

        /// <summary>
        /// Same as FindElement only returns null when not found instead of an exception.
        /// </summary>
        /// <param name="driver">current browser instance</param>
        /// <param name="by">The search string for finding element</param>
        /// <returns>Returns element or null if not found</returns>
        public static IWebElement FindElementSafe(this IWebElement  webElement, By by)
        {
            try
            {
                return webElement.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        /// <summary>
        /// Requires finding element by FindElementSafe(By).
        /// Returns T/F depending on if element is defined or null.
        /// </summary>
        /// <param name="element">Current element</param>
        /// <returns>Returns T/F depending on if element is defined or null.</returns>
        public static bool Exists(this IWebElement element)
        {
            if (element == null)
            { return false; }
            return true;
        }
    }
}