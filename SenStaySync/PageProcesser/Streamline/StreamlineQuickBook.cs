using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SenStaySync.Data;
using SenStaySync.Test;

namespace SenStaySync.PageProcesser.Streamline
{
    public class StreamlineQuickBook
    {
        public static void Process(IWebDriver driver, StreamlineAccount Account, ReservationInfo Reservation, bool IsTest = false)
        {
            string URL = "";
            if (IsTest)
            {
                URL = Mock.GetMock("StreamlineQuickBook1");
            }
            else
            {
                URL = @"https://www.resortpro.net/new/admin/index.html";
            }
            driver.GoTo(URL);

            var wait = driver.CreateWaitDriver();


            // All Quick links panel
            var divQuickLinks = driver.FindElement(By.Id("quick_links"));

            // Quick Book button
            var quickBookLink = divQuickLinks.FindAByHrefStartsWith("javascript:YAHOO.frontdesk.UpdateContent('ds_reservation_new.html',");
            quickBookLink.Click();
            driver.JustWait(2);


            // set type to AirBnb
            var typeOfReservationElement = new SelectElement(driver.FindElement(By.Id("type_id")));
            // 80 is AirBnb's magic number
            typeOfReservationElement.SelectByValue("80");
            driver.JustWait(1);


            var checkInDateFormated = Reservation.CheckIn.ToString("MM/dd/yyyy");
            var checkOutDateFormated = Reservation.CheckOut.ToString("MM/dd/yyyy");

            driver.ExecuteJS("document.getElementById('startdate').value = \"" + checkInDateFormated + "\";");
            driver.ExecuteJS("document.getElementById('enddate').value = \"" + checkOutDateFormated + "\";");



            var fieldsetElement = driver.FindElement(By.Id("unitnames"));
            var legendElement = fieldsetElement.FindElement(By.TagName("legend"));
            legendElement.Click();
            driver.JustWait(1);

            // Use filter to find unit by RoomTitle
            var filterUnitNameElement = driver.FindElement(By.Id("unit_name"));
            //filterUnitNameElement.SendKeys(Reservation.RoomTitle);

            Indexes.LoadSenStay();
            var SenStayID = Indexes.SenStay.GetSenStayIDByAirBnbName(Reservation.RoomTitle);

            filterUnitNameElement.SendKeys(SenStayID);


            //driver.FindElements(By.CssSelector(".yui-dt-rec"));

            driver.JustWait(3);
            var tbody = driver.FindElement(By.CssSelector("tbody.yui-dt-data"));
            var firstTr = tbody.FindElement(By.TagName("tr"));

            firstTr.Click();
            driver.JustWait(3);


            var changeRatesButton = driver.FindElement(By.CssSelector("[name=\"change_rates_button\"]"));
            changeRatesButton.Click();

            driver.JustWait(1);
            var authDialog = driver.FindElement(By.CssSelector("#AuthenticateDialog"));
            var passwordField = authDialog.FindElement(By.CssSelector("[name=\"password\"]"));

            driver.JustWait(1);
            passwordField.SendKeys(Account.Password);
            passwordField.Submit();
            driver.JustWait(1);



            //#AuthenticateDialog
            //name="password"

            //name="change_rates_button"

            //yui-dt-data
            //yui-dt-rec

            //unit_name

            //

            //Console.Write(quickBookLink.GetAttribute("href"));
            //Console.Read();
            //javascript:YAHOO.frontdesk.UpdateContent('ds_reservation_new.html', '');
        }
    }
}
