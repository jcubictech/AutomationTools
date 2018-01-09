using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenStaySync.Test
{
    class Mock
    {
        const string ASSETS_DIR = @"\Test\Assets\";

        private static Dictionary<string, string> Mocks = new Dictionary<string, string>()
        {
            { "ScrapeReservation1", "2015-09-30 AirBnb asset ScrapeReservation1.htm"},
            { "StreamlineLogin1", "2015-09-30 Streamline asset Login1.htm"},
            { "StreamlineQuickBook1", "2015-09-30 Streamline asset QuickBook1.htm"},
            

        };

        public static string GetMock(string MockName)
        {
            if (Mocks.ContainsKey(MockName))
            {
                var Mock = Mocks[MockName];
                return Config.I.AppDirictory + ASSETS_DIR + Mock;
            }
            return "";
        }

        public static ReservationInfo GetReservationItem()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ReservationInfo>(ITEM_JSON_1);
        }

        private const string ITEM_JSON_1 = "{\"Adult\":5,\"CheckIn\":\"2016-01-18T00:00:00\",\"CheckOut\":\"2016-01-24T00:00:00\",\"UnitID\":null,\"Price\":534.0,\"ArnbnbFee\":18.0,\"CleaningFee\":80.0,\"TotalAmount\":596.0,\"Code\":\"KQ5XQS\",\"ReservationId\":\"81749472\",\"ReservationCode\":\"KQ5XQS\",\"Dates\":\"Jan 18 - 24, 2016\",\"RoomId\":\"4574859\",\"RoomTitle\":\"Hollywood Studio Amazing View\",\"RoomAddress\":\"Sunset Boulevard 204 Los Angeles, CA 90028\",\"UserId\":\"35762561\",\"UserName\":\"Clare Woods\",\"PriceString\":\"614\",\"GuestFirstName\":\"Clare\",\"GuestLastName\":\"Woods\",\"GuestEmail\":\"clare-lxzrrhv6r6j6ihwu@guest.airbnb.com\"}";
        //private const string ITEM_JSON_1 = "{\"Adult\":5,\"CheckIn\":\"2016-01-18T00:00:00\",\"CheckOut\":\"2016-01-24T00:00:00\",\"UnitID\":null,\"Price\":534,\"ArnbnbFee\":18,\"CleaningFee\":80,\"TotalAmount\":596,\"User\":{\"FirstName\":\"Clare\",\"LastName\":\"Woods\",\"Email\":null},\"Code\":\"KQ5XQS\",\"Item\":{\"ReservationId\":\"81749472\",\"ReservationCode\":\"KQ5XQS\",\"Dates\":\"Jan 18 - 24, 2016\",\"RoomId\":\"4574859\",\"RoomTitle\":\"Hollywood Studio Amazing View\",\"RoomAddress\":\"Sunset Boulevard 204 Los Angeles, CA 90028\",\"UserId\":\"35762561\",\"UserName\":\"Clare Woods\",\"Price\":\"614\",\"Email\":\"clare-lxzrrhv6r6j6ihwu@guest.airbnb.com\"}}";


    }
}
