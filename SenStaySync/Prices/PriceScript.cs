using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SenStaySync.Data;

namespace SenStaySync.Prices
{
    using Data.Streamline;

    public class PriceScript : ISavable
    {
        public string UnitId;
        public DateTime Generated;
        public int StreamlineEditId;
        public List<PriceScriptCommand> Commands = new List<PriceScriptCommand>();

        public const string FILE_EXT = ".pricescript.json";

        public void Save()
        {
            this.SaveToFileAsJson(
                GetFilePath(),
                Newtonsoft.Json.Formatting.Indented);
        }

        public void Archive()
        {
            Temp.MoveToDirectory(new System.IO.FileInfo(GetFilePath()), Config.I.PriceOldScriptsDirectory);
        }

        public string GetFilePath()
        {
            return GetFilePath(UnitId, Generated);
        }

        public static string GetFilePath(string Unit, DateTime Generated)
        {
            return
                string.Format(@"{0}\{1}-{2}" + FILE_EXT,
                    Config.I.PriceScriptsDirectory,
                    Unit,
                    Generated.ToString("yyyy-MM-dd-HH-mm-ss"));
        }

        public static PriceScript Load(string Path)
        {
            try
            {
                var script = LoadUtils.Load<PriceScript>(Path, false);
                return script;
            }
            catch
            {
                N.Note("Can't read PriceScript " + Path);
                return null;
            }
        }

        public static List<PriceScript> LoadAllScripts()
        {
            var files = Temp.GetFileList("*" + FILE_EXT, Config.I.PriceScriptsDirectory);
            var list = new List<PriceScript>();
            foreach (var f in files)
            {
                var script = PriceScript.Load(f.FullName);
                if (script != null) list.Add(script);
            }
            return list;
        }

    }

    public static class PriceScripting
    {
        public static ScriptStatus CreateScript(
            UnitPriceStatus Status,
            FilteredPriceMap Prices,
            SenStayPropertyIndex Units,
            StreamlineSeasonGroup Seasons)
        {
            var script = new PriceScript()
            {
                UnitId = Status.Unit
            };
            var unitInfo = Units.GetBySenStayID(Status.Unit);
            if (unitInfo == null) return ScriptStatus.NOT_CREATED;

            script.StreamlineEditId = unitInfo.StreamlineEditID;

            if (Status.IsNew)
            {
                script.Commands.Add(
                    new PriceScriptCommand()
                    {
                        Type = PriceScriptCommand.TYPE_SEASON_GROUP,
                        SeasonGroupID = Seasons.ID,
                        StreamlineEditId = unitInfo.StreamlineEditID
                    }
                );
            }

            foreach (var p in Prices.Values)
            {
                script.Commands.Add(
                    new PriceScriptCommand()
                    {
                        Type = PriceScriptCommand.TYPE_PRICE,
                        Price = p.Price,
                        SeasonId = p.SeasonId
                    }
                );
            }

            script.Generated = DateTime.UtcNow;
            if (script.Commands.Count > 0)
            {
                script.Save();
                return ScriptStatus.CREATED;
            }
            return ScriptStatus.NOT_UPDATED;
        }
     
        public enum ScriptStatus
        {
            NOT_CREATED = 0,
            CREATED = 1,
            NOT_UPDATED = 2
        }   
    }

    public class PriceScriptCommand : SeasonPrice
    {
        public string Type;
        public int StreamlineEditId = 0;
        public int SeasonGroupID = 0;

        public const string TYPE_PRICE = "SetDailyPrice";
        public const string TYPE_SEASON_GROUP = "SetSeasonsGroup";
    }
    
}
