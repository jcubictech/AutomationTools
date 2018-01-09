namespace SenStaySync.Tools
{
    using System.Collections.Generic;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Remote;

    public class SeleniumProxy
    {
        public static FirefoxProfile GetFirefoxProfileWithProxy(List<string> proxyAddress, int proxyNum = 0)
        {
            var profile = new FirefoxProfile();

            if (!Config.I.UseProxy || proxyAddress == null || proxyAddress.Count <= 0)
                return profile;

            return GetFirefoxProfileWithProxy(proxyAddress[proxyNum]);
        }

        public static FirefoxProfile GetFirefoxProfileWithProxy(string proxy)
        {
            var profile = new FirefoxProfile();

            if (!Config.I.UseProxy || string.IsNullOrEmpty(proxy))
                return profile;

            try
            {
                var pr = new Proxy
                {
                    HttpProxy = proxy,
                    FtpProxy = proxy,
                    SslProxy = proxy
                };

                profile.SetProxyPreferences(pr);
                profile.SetPreference("network.proxy.type", 1);
                profile.SetPreference("network.proxy.http", proxy.ExtractIPAddress());
                profile.SetPreference("network.proxy.http_port", proxy.ExtractPortNumber());
                profile.SetPreference("network.proxy.ssl", proxy.ExtractIPAddress());
                profile.SetPreference("network.proxy.ssl_port", proxy.ExtractPortNumber());
            }
            catch
            {
                // ignored
            }

            return profile;
        }

        public static ChromeOptions GetChromeOptionsWithProxy(List<string> proxyAddress, int proxyNum = 0)
        {
            ChromeOptions chromeOptions = new ChromeOptions();

            if (!Config.I.UseProxy || proxyAddress == null || proxyAddress.Count <= 0)
                return chromeOptions;

            return GetChromeOptionsWithProxy(proxyAddress[proxyNum]);
        }

        public static ChromeOptions GetChromeOptionsWithProxy(string proxy)
        {
            var chromeOptions = new ChromeOptions();

            if (!Config.I.UseProxy || string.IsNullOrEmpty(proxy))
                return chromeOptions;

            try
            {
                var pr = new Proxy
                {
                    HttpProxy = proxy,
                    FtpProxy = proxy,
                    SslProxy = proxy,
                    Kind = ProxyKind.Manual
                };
                
                ////chromeOptions
                chromeOptions.Proxy = pr;
                chromeOptions.AddArgument("enable-automation");
                chromeOptions.AddArgument("disable-infobars");
                
            }
            catch
            {
                // ignored
            }

            return chromeOptions;
        }
    }
}