using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SenStaySync.PageProcesser.AirBnb;

namespace SenStaySync.Test.Scripts
{
    public class Scrape1
    {
        public static void Process()
        {
            var Dr = SeleniumFactory.GetFirefoxDriver();
            var Account = Data.AirBnbAccounts.GetAccountByEmail("sashaairbnb1@gmail.com");
            //AirBnbScrapeReservation.Process(Dr, Account, true);

            var Items = PageProcesser.AirBnb.AirBnbScrapeReservation.GetAllReservationItems(Dr, true);
            var Item = PageProcesser.AirBnb.AirBnbScrapeReservation.GetFullReservationInfo(Items[0], Dr, Account, true);


            Console.Write(Item.ToJson());
            Console.Read();

        }

        public static void ProcessProduction()
        {
            var Dr = SeleniumFactory.GetFirefoxDriver();
            var Account = Data.AirBnbAccounts.GetAccountByEmail("sashaairbnb1@gmail.com");
            //AirBnbScrapeReservation.Process(Dr, Account, true);

            PageProcesser.AirBnb.AirBnbLogin.Process(Dr, Account);
            var Items = PageProcesser.AirBnb.AirBnbScrapeReservation.GetAllReservationItems(Dr, false);
            var Item = PageProcesser.AirBnb.AirBnbScrapeReservation.GetFullReservationInfo(Items[0], Dr, Account, false);

            var json = Item.ToJson();
            Console.Write(json);
            Console.Read();
        }


    }
}
