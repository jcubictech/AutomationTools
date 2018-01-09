namespace SenStaySync.PageProcesser.AirBnb
{
    using System;
    using System.Diagnostics;
    using Data;
    using OpenQA.Selenium;

    public class AirBnbLogin
    {
        private const string ProxyErrorMessage = "The proxy server is refusing connections";
        private const string TimedOutErrorMessage = "The connection has timed out";

        public static LoginResult Process(IWebDriver driver, AirBnbAccount Account)
        {
            var loginResult = new LoginResult();

            try
            {
                try
                {
                    driver.GoTo("https://www.airbnb.com/login");

                    var emailInput = driver.FindElement(By.Id("signin_email"));
                    var passwordInput = driver.FindElement(By.Id("signin_password"));
                    var submit = driver.FindElement(By.Id("user-login-btn"));

                    emailInput.SendKeys(Account.Email);
                    passwordInput.SendKeys(Account.Password);
                    passwordInput.Submit();
                }
                catch
                {
                    try
                    {
                        var errorElement = driver.FindElement(By.Id("errorTitleText"));
                        
                        if(string.Compare(errorElement.Text, ProxyErrorMessage, true) == 0)
                        {
                            loginResult.Status = AirBnbLoginStatus.ProxyError;
                            loginResult.Message = ProxyErrorMessage;
                            return loginResult;
                        }
                        else if(string.Compare(errorElement.Text, TimedOutErrorMessage, true) == 0)
                        {
                            loginResult.Status = AirBnbLoginStatus.TimedOut;
                            loginResult.Message = TimedOutErrorMessage;
                            return loginResult;
                        }
                        
                    }
                    catch (Exception)
                    {
                        loginResult.Status = AirBnbLoginStatus.PageError;
                        return loginResult;
                    }
                }

                driver.JustWait(1);

                try
                {
                    var alert = driver.FindElement(By.CssSelector(".alert.alert-notice.alert-info")).Text;
                    if (!string.IsNullOrEmpty(alert))
                    {
                        loginResult.Status = AirBnbLoginStatus.Failed;
                        loginResult.Message = alert;
                        return loginResult;
                    }
                }
                catch
                {
                    // ignored
                }

                try
                {
                    var a = driver.FindElement(By.XPath("//*[contains(text(), 'Unable to perform')]"));
                    if (a != null)
                    {
                        loginResult.Status = AirBnbLoginStatus.Unable;
                        return loginResult;
                    }
                }
                catch
                {
                    // ignored
                }

                driver.JustWait(5);

                if (driver.Url.Contains(@"airbnb.com/dashboard"))
                {
                    loginResult.Status = AirBnbLoginStatus.Sucess;
                    return loginResult;
                }

                if (driver.Url.Contains(@"airbnb.com/airlock"))
                {
                    loginResult.Status = AirBnbLoginStatus.VerifyUser;
                    return loginResult;
                }

                if (driver.Url.Contains(@"airbnb.com/remember_browser"))
                {
                    try
                    {
                        var siteContent = driver.FindElement(By.Id("site-content"));
                        var button = siteContent.FindElement(By.TagName("button"));
                        button.Click();
                        driver.JustWait(2);
                    }
                    catch (Exception)
                    {
                        loginResult.Status = AirBnbLoginStatus.VerifyBrowser;
                        return loginResult;
                    }
                }

                loginResult.Status = AirBnbLoginStatus.Sucess;
            }
            catch (Exception)
            {
                if (driver == null)
                {
                    loginResult.Status = AirBnbLoginStatus.Failed;
                    return loginResult;
                }

                // As per the existing program this code will never be hit, Hence commenting the same out. Since the same is used by different projects.

                //try
                //{
                //    var errorElement = driver.FindElement(By.Id("errorTitleText"));
                //    if (errorElement.Text == "The proxy server is refusing connections")
                //    {
                //        loginResult.Status = AirBnbLoginStatus.ProxyError;
                //        loginResult.Message = "The proxy server is refusing connections";
                //        return loginResult;
                //    }
                //}
                //catch (Exception)
                //{
                //    loginResult.Status = AirBnbLoginStatus.Failed;
                //    return loginResult;
                //}
            }

            return loginResult;
        }
    }
}