namespace SenStaySync.PageProcesser.Streamline
{
    using Data;
    using OpenQA.Selenium;
    using System;
    using Test;

    public class StreamlineLogin
    {
        public static void Process(IWebDriver driver, StreamlineAccount account, bool isTest = false)
        {
                var url = isTest ? Mock.GetMock("StreamlineLogin1") : @Config.I.StreamLineLoginPageURL;
                driver.GoTo(url);

                var loginInput = driver.FindElement(By.Name("user_login"));

                var passwordInput = driver.FindElement(By.Name("user_password"));
                var submit = driver.FindElement(By.Name("submit_button"));
                driver.JustWait(3);

                loginInput.SendKeys(account.Login);
                passwordInput.SendKeys(account.Password);
                driver.JustWait(3);
                submit.Click();

                driver.JustWait(5);   
        }

        public static bool ProcessLogin(IWebDriver driver, StreamlineAccount account, bool isTest = false)
        {
            bool isSuccessfulLogin = true;
            try
            { 
                var url = isTest ? Mock.GetMock("StreamlineLogin1") : @Config.I.StreamLineLoginPageURL;
                driver.GoTo(url);

                var loginInput = driver.FindElement(By.Name("user_login"));

                var passwordInput = driver.FindElement(By.Name("user_password"));
                var submit = driver.FindElement(By.Name("submit_button"));
                driver.JustWait(3);

                loginInput.SendKeys(account.Login);
                passwordInput.SendKeys(account.Password);
                driver.JustWait(3);
                submit.Click();
                driver.JustWait(5);
            }
            catch
            {
                isSuccessfulLogin = false;
            }
            return isSuccessfulLogin;
        }

        public static void CheckNewsDialogPopup(IWebDriver driver)
        {
            try
            {
                IWebElement closeLink = driver.FindElement(By.XPath("//div[@id ='NewsDialog']//a[@class='container-close']"));
                closeLink.Click();
                N.Note("News dialog has been closed");
            }
            catch (NoSuchElementException)
            {
                N.Note("News dialog was not found");
                
            }
        }
    }
}