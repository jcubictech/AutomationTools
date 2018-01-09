namespace SenStaySync.Data
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class StreamlineSeason
    {
        public int Day;

        public int GroupID;
        public int ID;
        public int Month;

        public string Name;
    }

    public class StreamlineSeasonGroup : ISavable
    {
        public DateTime ChangedDate;
        public int ID;
        public List<StreamlineSeason> Seasons = new List<StreamlineSeason>();

        public void Save()
        {
            ChangedDate = DateTime.UtcNow;
            this.SaveToFileAsJson(Config.I.DailySeasons, Formatting.Indented);
        }

        public void Add(int ID, string Name)
        {
            var season = new StreamlineSeason
            {
                ID = ID,
                Name = Name,
                Month = Convert.ToInt32(Name.Substring(0, 2)),
                Day = Convert.ToInt32(Name.Substring(3, 2)),
                GroupID = this.ID
            };
            Seasons.Add(season);
        }

        public StreamlineSeason GetSeasonByDay(int Month, int Day)
        {
            foreach (var i in Seasons)
            {
                if (i.Day == Day && i.Month == Month) return i;
            }
            return null;
        }

        public static StreamlineSeasonGroup Load()
        {
            return LoadUtils.Load<StreamlineSeasonGroup>(Config.I.DailySeasons);
        }
    }

    public class Season
    {
        public DateTime SeasonStartDate { get; set; }
        public DateTime SeasonEndDate { get; set; }
        public string SeasonId { get; set; }
    }
}