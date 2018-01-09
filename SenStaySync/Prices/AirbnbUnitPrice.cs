namespace SenStaySync.Prices
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class AirbnbUnitPrice
    {
    }

    public class AirbnbUnitDailyPrice
    {
        public int Day = 1;
        public int Month = 1;
        public int Price = 0; // example = 100 ; means $100 
        public int RoomID;
        public string UnitSenStayID = ""; // example LA019
        public int Year = 1;

        public string MonthValue()
        {
            return Year + "-" + Month;
        }

        public DateTime DU()
        {
            return new DateTime(Year, Month, Day);
        }

        public string Log()
        {
            return string.Format("{0}-{1}-{2} = {3}", Year, Month, Day, Price);
        }
    }

    public class AirbnbUnitDailyPriceList : List<AirbnbUnitDailyPrice>, ISavable
    {
        public void Save()
        {
            this.SaveToFileAsJson("ny022.json", Formatting.Indented);
        }

        public static AirbnbUnitDailyPriceList Load()
        {
            return LoadUtils.Load<AirbnbUnitDailyPriceList>("ny022.json");
        }
    }
}