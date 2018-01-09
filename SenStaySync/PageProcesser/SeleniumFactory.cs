namespace SenStaySync
{
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.Remote;
    using OpenQA.Selenium.Chrome;

    /// <summary>
    /// Selenium factory.
    /// </summary>
    public class SeleniumFactory
    {
        /// <summary>
        /// Get the firefox driver.
        /// </summary>
        /// <param name="profile">Firefox profile.</param>
        /// <returns>Remote web driver.</returns>
        public static RemoteWebDriver GetFirefoxDriver(FirefoxProfile profile = null)
        {
            if (profile == null)
            {
                return new FirefoxDriver();
            }
            else
            {
                return new FirefoxDriver(profile);
            }
        }

        public static RemoteWebDriver GetChromeDriver(ChromeOptions chromeOptions = null)
        {
            if (chromeOptions == null)
            {
                return new ChromeDriver();
            }
            else
            {
                return new ChromeDriver(chromeOptions);
            }
        }
    }
}