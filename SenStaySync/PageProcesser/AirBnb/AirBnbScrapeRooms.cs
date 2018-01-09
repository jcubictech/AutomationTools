namespace SenStaySync.PageProcesser.AirBnb
{
    using System;
    using System.Collections.Generic;
    using Data;
    using OpenQA.Selenium;

    public static class AirBnbScrapeRooms
    {
        public static AirBnbAccountRooms GetRoomList(IWebDriver driver, AirBnbAccount account, bool isTest = false)
        {
            var accountRooms = new AirBnbAccountRooms
            {
                AccountEmail = account.Email
            };

            NavigateToListing(driver);
            accountRooms.Rooms = ScrapeRooms(driver, account);
            return accountRooms;
        }

        private static void NavigateToListing(IWebDriver driver)
        {
            driver.GoTo("https://www.airbnb.com/rooms");
            var wait = driver.CreateWaitDriver();

            // Wait until footer loaded
            wait.CssSelector("#footer");
            driver.JustWait(5);
        }

        private static List<AirBnbRoom> ScrapeRooms(IWebDriver driver, AirBnbAccount account)
        {
            var list = new List<AirBnbRoom>();
            var h4List = driver.FindElements(By.CssSelector("span.h4"));

            foreach (var h4Span in h4List)
            {
                try
                {
                    var a = h4Span.FindElement(By.TagName("a"));
                    var href = a.GetAttribute("href");
                    var roomId = href.ExtractNumber();

                    var spans = h4Span.FindElements(By.TagName("span"));
                    var roomTitle = "";
                    foreach (var span in spans)
                    {
                        try
                        {
                            var text = (span.Text + "").Trim();
                            if (string.IsNullOrWhiteSpace(text))
                                continue;

                            roomTitle = text;
                            break;
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    Console.WriteLine(account.Email + " - " + roomTitle + " - " + roomId);
                    list.Add(new AirBnbRoom
                    {
                        AccountEmail = account.Email,
                        RoomID = roomId,
                        RoomTitle = roomTitle
                    });
                }
                catch
                {
                    // ignored
                }
            }

            return list;
        }
    }
}