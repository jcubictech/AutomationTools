namespace SenStaySync.Data
{
    using System;
    using System.Collections.Generic;
    using PageProcesser.AirBnb;

    public class LoginAttempt
    {
        public LoginAttempt(string login, AirBnbLoginStatus loginStatus, string message, string proxy)
        {
            Login = login;
            LoginStatus = loginStatus;
            Message = message;
            Proxy = proxy;
            LoginTime = DateTime.UtcNow;
        }

        public string Message { get; set; }
        public string Proxy { get; set; }
        public string Login { get; set; }
        public DateTime LoginTime { get; set; }
        public AirBnbLoginStatus LoginStatus { get; set; }
    }

    public class LoginStatistics
    {
        public int LoginCount { get; set; }
        public int ErrorsCount { get; set; }
        public Dictionary<string, string> ErrorLogins { get; set; }
        public IEnumerable<string> InvalidProxies { get; set; }
    }
}