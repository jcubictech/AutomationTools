namespace SenStaySync.PageProcesser.AirBnb
{
    using System.ComponentModel;

    public enum AirBnbLoginStatus
    {
        [Description("Login failed")]
        Failed = 0,
        [Description("Successfull login")]
        Sucess = 1,
        [Description("Unable to login")]
        Unable = 2,
        [Description("Proxy error")]
        ProxyError = 3,
        [Description("Verify user error")]
        VerifyUser = 4,
        [Description("Verify browser error")]
        VerifyBrowser = 5,
        [Description("Page error")]
        PageError = 6,
        [Description("Timed out")]
        TimedOut = 7 
    }
}