using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SenStaySync.Data;
using OpenQA.Selenium.Remote;
using System.Collections.ObjectModel;
using SenStaySync.Test;
using SenStaySync;

namespace SenStaySync.PageProcesser.AirBnb
{
    public class AirBnbScrapeReservation
    {
        /*
        public static ReservationInfo Process(RemoteWebDriver Driver, AirBnbAccount Account, bool IsTest = false)
        {
            ReservationInfo info = GetReservationInfo(Driver, items[0], IsTest);
            return info;
        }
        //*/

        public static List<ReservationInfo> ScrapeReservationList(RemoteWebDriver Driver, AirBnbAccount Account, bool IsTest = false)
        {
            var items = GetAllReservationItems(Driver, IsTest);
            return items;
        }

        public static ReservationInfo GetFullReservationInfo(ReservationInfo Item, RemoteWebDriver Driver, AirBnbAccount Account, bool IsTest = false)
        {
            return GetReservationInfo(Driver, Item, IsTest);
        }


        public static List<ReservationInfo> GetAllReservationItems(RemoteWebDriver Driver, bool IsTest = false)
        {
            //Driver.Navigate().GoToUrl(@"https://airbnb.com/my_reservations?all=1");

            string URL = "";
            if (IsTest)
            {
                URL = Mock.GetMock("ScrapeReservation1");
            }
            else
            {
                URL = @"https://airbnb.com/my_reservations?all=1";
            }
            Driver.GoTo(URL);

            //IWait<IWebDriver> wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30.00));
            //wait.Until(driver1 => ((IJavaScriptExecutor)Driver).ExecuteScript("return document.readyState").Equals("complete"));


            //_wait.Until(ExpectedConditions.ElementExists(By.ClassName("table")));
            //var table = _driver.FindElementByCssSelector("table");

            var table = Driver.FindElement(By.CssSelector("table"), Config.I.RegularWaitSeconds);

            // достаем данные из таблицы
            ReadOnlyCollection<IWebElement> trs = table.FindElements(By.CssSelector("tr"));
            List<ReservationInfo> resItems = new List<ReservationInfo>();

            foreach (var tr in trs)
            {
                // пропускаем строку если в нет reservation id
                if (String.IsNullOrEmpty(tr.GetAttribute("data-reservation-id")))
                    continue;

                ReservationInfo resItem = new ReservationInfo();
                resItem.ReservationId = tr.GetAttribute("data-reservation-id");
                resItem.ReservationCode = tr.GetAttribute("data-reservation-code");

                var Email = "";
                var Aemail = tr.FindAByHrefStartsWith("mailto:");
                if (Aemail != null)
                {
                    Email = Aemail.GetAttribute("href").Substring(7);
                }
                resItem.GuestEmail = Email;

                // получаем столбцы строки
                ReadOnlyCollection<IWebElement> tds = tr.FindElements(By.CssSelector("td"));

                if (tds.Count > 0)
                {
                    // парсим второй столбец с датой и комнатой
                    string[] td1Texts = tds[1].Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    resItem.Dates = td1Texts[0];
                    resItem.RoomTitle = td1Texts[1];
                    string[] roomHrefParts = tds[1].FindElement(By.CssSelector("a")).GetAttribute("href").Split(new char[] { '/' });
                    resItem.RoomId = roomHrefParts[roomHrefParts.Length - 1];
                    resItem.RoomAddress = String.Format("{0} {1}", td1Texts[2], td1Texts[3]);

                    // парсим третий столбец с пользователем
                    var userLink = tds[2].FindElements(By.CssSelector("a"))[1];
                    string[] userLinkParts = userLink.GetAttribute("href").Split(new char[] { '/' });
                    resItem.UserId = userLinkParts[userLinkParts.Length - 1];
                    resItem.UserName = userLink.Text;

                    // парсим четвертый столбец с ценой
                    resItem.PriceString = tds[3].Text.Split(new char[] { ' ' })[0].Substring(1);
                }

                resItems.Add(resItem);
            }

            //Drive.Close();
            return resItems;
        }

        public static ReservationInfo GetReservationInfo(RemoteWebDriver Driver, ReservationInfo info, bool IsTest = false)
        {
            string code = info.ReservationCode;
            // на страницу конкретного резерва
            //Driver.Navigate().GoToUrl(String.Format("https://www.airbnb.com/reservation/itinerary?code={0}", code));
            Driver.GoTo(String.Format("https://www.airbnb.com/reservation/itinerary?code={0}", code));

            WebDriverWait _wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 30));

            // ждем пока загрузится
            _wait.Until(ExpectedConditions.ElementExists(By.Id("site-content")));

            //info.Item = Item;
            info.Code = code;

            // получаем даты и кол-во гостей
            var divTable = Driver.FindElementByCssSelector("div[class=\"row row-table row-space-2\"]");
            ReadOnlyCollection<IWebElement> dateDivs = divTable.FindElements(By.CssSelector("div[class=\"col-4 col-top\"]"));

            ReadOnlyCollection<IWebElement> checkInDivs = dateDivs[0].FindElements(By.CssSelector("div"));
            info.CheckIn = DateTime.Parse(checkInDivs[0].Text);
            ReadOnlyCollection<IWebElement> checkOutDivs = dateDivs[1].FindElements(By.CssSelector("div"));
            info.CheckOut = DateTime.Parse(checkOutDivs[0].Text);

            ReadOnlyCollection<IWebElement> numDivs = divTable.FindElements(By.CssSelector("div[class=\"col-2 col-bottom\"]"));
            ReadOnlyCollection<IWebElement> adultDivs = numDivs[1].FindElements(By.CssSelector("div"));
            info.Adult = Int32.Parse(adultDivs[0].Text);

            // получаем денежные данные
            var payTable = Driver.FindElementByCssSelector("table[class=\"table payment-table\"]");
            ReadOnlyCollection<IWebElement> trs = payTable.FindElements(By.CssSelector("tr"));
            string strPrice = trs[0].FindElement(By.CssSelector("td")).Text;
            info.Price = Int32.Parse(strPrice.Substring(1));
            string strCleaning = trs[1].FindElement(By.CssSelector("td")).Text;
            info.CleaningFee = Int32.Parse(strCleaning.Substring(1));
            string strArnbnbFee = trs[2].FindElement(By.CssSelector("td")).Text;
            info.ArnbnbFee = Int32.Parse(strArnbnbFee.Substring(2).Substring(0, strArnbnbFee.Length - 3));
            string strTotal = trs[3].FindElement(By.CssSelector("td")).Text;
            info.TotalAmount = Int32.Parse(strTotal.Substring(1));

            // получаем имя юзера
            var mediaBodyDiv = Driver.FindElementByCssSelector("div[class=\"media-body\"]");
            var userLink = mediaBodyDiv.FindElement(By.CssSelector("a"));
            string[] usernameParts = userLink.Text.Split(new char[] { ' ' });
            info.GuestFirstName = usernameParts[0];
            info.GuestLastName = usernameParts[1];

            return info;
        }

        //https://www.airbnb.com/reservation/itinerary?code=2ZDBJY
        public static void ParseReservationByCode(RemoteWebDriver Driver, string Code, bool IsTest = false)
        {
            var item = new ReservationInfo();


            item.Code = Code;
            //item.


            Driver.GoTo(String.Format("https://www.airbnb.com/reservation/itinerary?code={0}", Code));
            WebDriverWait _wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 30));
            _wait.Until(ExpectedConditions.ElementExists(By.Id("site-content")));

            // получаем даты и кол-во гостей
            var divTable = Driver.FindElementByCssSelector("div[class=\"row row-table row-space-2\"]");
            ReadOnlyCollection<IWebElement> dateDivs = divTable.FindElements(By.CssSelector("div[class=\"col-4 col-top\"]"));

            ReadOnlyCollection<IWebElement> checkInDivs = dateDivs[0].FindElements(By.CssSelector("div"));
            item.CheckIn = DateTime.Parse(checkInDivs[0].Text);
            ReadOnlyCollection<IWebElement> checkOutDivs = dateDivs[1].FindElements(By.CssSelector("div"));
            item.CheckOut = DateTime.Parse(checkOutDivs[0].Text);

            ReadOnlyCollection<IWebElement> numDivs = divTable.FindElements(By.CssSelector("div[class=\"col-2 col-bottom\"]"));
            ReadOnlyCollection<IWebElement> adultDivs = numDivs[1].FindElements(By.CssSelector("div"));
            item.Adult = Int32.Parse(adultDivs[0].Text);



            // Room Title
            var divs1 = Driver.FindElementsByCssSelector("div[class=\"col-6\"]");
            var a = divs1[0].FindAByHrefStartsWith("/rooms/");
            item.RoomTitle = a.Text;

            //Guest name, phonenumber, email
            var a2 = divs1[1].FindAByHrefStartsWith("/users/show/");


            // получаем денежные данные
            var payTable = Driver.FindElementByCssSelector("table[class=\"table payment-table\"]");
            ReadOnlyCollection<IWebElement> trs = payTable.FindElements(By.CssSelector("tr"));
            string strPrice = trs[0].FindElement(By.CssSelector("td")).Text;
            item.Price = Int32.Parse(strPrice.Substring(1));
            string strCleaning = trs[1].FindElement(By.CssSelector("td")).Text;
            item.CleaningFee = Int32.Parse(strCleaning.Substring(1));
            string strArnbnbFee = trs[2].FindElement(By.CssSelector("td")).Text;
            item.ArnbnbFee = Int32.Parse(strArnbnbFee.Substring(2).Substring(0, strArnbnbFee.Length - 3));
            string strTotal = trs[3].FindElement(By.CssSelector("td")).Text;
            item.TotalAmount = Int32.Parse(strTotal.Substring(1));

            // получаем имя юзера
            var mediaBodyDiv = Driver.FindElementByCssSelector("div[class=\"media-body\"]");
            var userLink = mediaBodyDiv.FindElement(By.CssSelector("a"));
            string[] usernameParts = userLink.Text.Split(new char[] { ' ' });
            item.GuestFirstName = usernameParts[0];
            item.GuestLastName = usernameParts[1];


        }
    }


}
