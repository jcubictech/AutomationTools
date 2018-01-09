namespace SenStaySync.Prices
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Data;
    using Data.Streamline;
    using Tools;

    public static class PriceParsing
    {
        public static void ParseFolder(string Path)
        {
            var seasons = StreamlineSeasonGroup.Load();
            var streamlineCollection = StreamlinePropertyCollection.Load();
            var units = streamlineCollection.CreateIndex();
            var map = new PriceMap();
            var files = Temp.GetFileList(@"*.csv", Path);
            foreach (var file in files)
            {
                ProcessSourceFile(file, units, seasons, map);
            }

            CreatePriceScripts(map, units, seasons);


            var airbnbRooms = AirBnbRoomMap.Load();
            PropertyMap.SetMap(airbnbRooms, streamlineCollection);
            CreateAirbnbNY022(map, airbnbRooms);
        }

        public static void ProcessSourceFile(FileInfo File,
            SenStayPropertyIndex Units,
            StreamlineSeasonGroup Seasons,
            PriceMap map)
        {
            N.Note("Parsing " + File.FullName);
            var priceData = CSVReader.Read4columnCSV(File.FullName);
            ExtractMap(map, priceData);
            //Temp.MoveToDirectory(File, Config.I.PriceOldSourceDirectory);
            N.Note("Moving source to the archive " + File.FullName);
        }

        public static void CreatePriceScripts(PriceMap Map,
            SenStayPropertyIndex Units,
            StreamlineSeasonGroup Seasons)
        {
            var UnitIDs = Map.GetUnitIDs();
            foreach (var UnitId in UnitIDs)
            {
                var status = UnitPriceStatus.Load(UnitId);
                var prices = Map.GetPricesByUnitFilteredByStatus(
                    UnitId, Units, Seasons, status);

                var scriptCreated = PriceScripting.CreateScript(status, prices, Units, Seasons);
                if (scriptCreated == PriceScripting.ScriptStatus.NOT_CREATED)
                {
                    N.Note("PriceScript for " + UnitId + " NOT created");
                    continue;
                }
                if (scriptCreated == PriceScripting.ScriptStatus.NOT_UPDATED)
                {
                    //N.Note("PriceScript for " + UnitId + " NOT updated");
                    continue;
                }
                N.Note("PriceScript for " + UnitId + " created");
                status.TimeStamp = DateTime.UtcNow;
                status.Save();
            }
        }


        public static void CreateAirbnbNY022(PriceMap Map, AirBnbRoomMap AirbnbRooms)
        {
            if (!Map.Data.ContainsKey("NY022"))
                return;

            var list = Map.Data["NY022"];
            var unit = PropertyMap.GetBySenstayID("NY022");

            if (unit == null)
                return;

            var result = new AirbnbUnitDailyPriceList();
            foreach (var item in list)
            {
                result.Add(new AirbnbUnitDailyPrice
                {
                    Month = item.Month,
                    Day = item.Day,
                    Year = 2016,
                    Price = item.Price,
                    RoomID = unit.AirbnbID,
                    UnitSenStayID = "NY022"
                });
            }
            result.Save();
        }

        public static void ExtractMap(PriceMap map, List<SourcePriiceItem> priceData)
        {
            foreach (var d in priceData)
            {
                var dateStr = d.Date;
                var priceStr = d.Prce;
                var unitStr = d.Unit;
                var timeStampStr = d.TimeStamp;

                var date = DateTime.UtcNow;
                var timeStamp = DateTime.UtcNow;
                var Price = 0;

                if (!DateTime.TryParse(dateStr, new CultureInfo("en-US").DateTimeFormat, DateTimeStyles.None, out date))
                {
                    continue;
                }

                if (!DateTime.TryParse(timeStampStr, out timeStamp))
                {
                    continue;
                }

                if (!int.TryParse(priceStr, out Price))
                {
                    continue;
                }

                map.Add(unitStr, date.Month, date.Day, Price, timeStamp);
            }
        }
    }
}