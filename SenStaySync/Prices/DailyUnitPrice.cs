namespace SenStaySync.Prices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Data.Streamline;

    public class DailyPrice
    {
        public int Day = 1;
        public int Month = 1;
        public int Price; // example = 100 ; means $100 

        /// <summary>
        /// Timestamp.
        /// </summary>
        public DateTime Tiemstamp;

        public string UnitSenStayID = ""; // example LA019

        public string GetKey()
        {
            return string.Format("{0}-{1}", Month, Day);
        }
    }

    public class AdoptedDailyPrice : SeasonPrice
    {
        public DateTime TimeStamp;
        public int UnitStrreamlineEditId;
    }

    public class SeasonPrice
    {
        public int Price;
        public int SeasonId;
    }

    public static class PriceProcesser
    {
        public static AdoptedDailyPrice Convert(DailyPrice p, SenStayPropertyIndex Units, StreamlineSeasonGroup Seasons)
        {
            var season = Seasons.GetSeasonByDay(p.Month, p.Day);
            var unit = Units.GetBySenStayID(p.UnitSenStayID);
            if (unit == null || season == null) return null;
            return new AdoptedDailyPrice
            {
                UnitStrreamlineEditId = unit.StreamlineEditID,
                Price = p.Price,
                SeasonId = season.ID,
                TimeStamp = p.Tiemstamp
            };
        }
    }

    public class PriceMap
    {
        public Dictionary<string, List<DailyPrice>> Data = new Dictionary<string, List<DailyPrice>>();


        public List<string> GetUnitIDs()
        {
            return Data.Keys.ToList();
        }

        public void Add(string SenStayID, int Month, int Day, int Price, DateTime Timestamp)
        {
            if (!Data.ContainsKey(SenStayID)) Data.Add(SenStayID, new List<DailyPrice>());

            Data[SenStayID].Add(new DailyPrice
            {
                UnitSenStayID = SenStayID,
                Day = Day,
                Month = Month,
                Price = Price,
                Tiemstamp = Timestamp
            });
        }

        public List<AdoptedDailyPrice> GetPricesByUnit(string SenStayID, SenStayPropertyIndex Units,
            StreamlineSeasonGroup Seasons)
        {
            if (!Data.ContainsKey(SenStayID)) return null;
            var prices = Data[SenStayID];

            var list = new List<AdoptedDailyPrice>();
            foreach (var p in prices)
            {
                var item = PriceProcesser.Convert(p, Units, Seasons);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            return list;
        }


        public FilteredPriceMap GetPricesByUnitFilteredByStatus(
            string SenStayID,
            SenStayPropertyIndex Units,
            StreamlineSeasonGroup Seasons,
            UnitPriceStatus Status)
        {
            if (!Data.ContainsKey(SenStayID)) return null;
            var prices = Data[SenStayID];

            var NewsetTimestamp = Status.LastSyncTimeStamp;
            var list = new FilteredPriceMap();
            foreach (var p in prices)
            {
                if (p.Tiemstamp <= Status.LastSyncTimeStamp) continue;
                var key = p.GetKey();

                if (p.Tiemstamp > NewsetTimestamp) NewsetTimestamp = p.Tiemstamp;

                var item = PriceProcesser.Convert(p, Units, Seasons);
                if (item == null) continue;

                if (list.ContainsKey(key))
                {
                    if (list[key].TimeStamp < p.Tiemstamp)
                    {
                        list[key] = PriceProcesser.Convert(p, Units, Seasons);
                    }
                }
                else
                {
                    list.Add(key, PriceProcesser.Convert(p, Units, Seasons));
                }
            }
            Status.LastSyncTimeStamp = NewsetTimestamp;
            return list;
        }
    }

    public static class PricesExtenstions
    {
        public static List<SeasonPrice> GetSeasonPriceList(this List<AdoptedDailyPrice> prices)
        {
            var list = new List<SeasonPrice>();
            foreach (var p in prices)
            {
                list.Add(new SeasonPrice
                {
                    Price = p.Price,
                    SeasonId = p.SeasonId
                });
            }
            return list;
        }
    }

    public class FilteredPriceMap : Dictionary<string, AdoptedDailyPrice>
    {
    }
}