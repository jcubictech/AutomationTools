using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenStaySync.Prices
{
    public class UnitPriceStatus : ISavable
    {
        public string Unit;
        public DateTime LastSyncTimeStamp;
        public DateTime TimeStamp;

        public void Save()
        {
            this.TimeStamp = DateTime.UtcNow;
            this.SaveToFileAsJson(
                GetFilePath(),
                Newtonsoft.Json.Formatting.Indented);
        }

        public static UnitPriceStatus Load(string Unit)
        {
            try
            {
                var status = LoadUtils.Load<UnitPriceStatus>(GetFilePath(Unit), false);
                if (status == null || status.Unit == null)
                {
                    throw new Exception();
                }
                return status;
            }
            catch
            {
                return new UnitPriceStatus() { Unit = Unit, LastSyncTimeStamp = DateTime.MinValue };
            }
        }

        public string GetFilePath()
        {
            return GetFilePath(Unit);
        }

        public static string GetFilePath(string Unit)
        {
            return Config.I.PriceStatusDirectory + @"\" + Unit + ".pricestatus.json";
        }

        public bool IsNew
        {
            get {
                return !Temp.Exists(GetFilePath());
                //return LastSyncTimeStamp == DateTime.MinValue;
            }
        }

    }

}
