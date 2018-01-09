namespace SenStaySync.Data.Streamline
{
    using System.Collections.Generic;

    public class SenStayPropertyIndex
    {
        private readonly Dictionary<string, StreamlinePropertyInfo> AirBnbNameToSenStayID =
            new Dictionary<string, StreamlinePropertyInfo>();

        private readonly Dictionary<string, StreamlinePropertyInfo> SenStayIDToAirBnbName =
            new Dictionary<string, StreamlinePropertyInfo>();

        public string GetSenStayIDByAirBnbName(string AirBnbName)
        {
            var Item = GetByAirBnbName(AirBnbName);
            return Item != null ? Item.SenStayID : "";
        }

        public StreamlinePropertyInfo GetByAirBnbName(string AirBnbName)
        {
            var Key = AirBnbName.SafeTrim();
            if (AirBnbNameToSenStayID.ContainsKey(Key))
            {
                return AirBnbNameToSenStayID[Key];
            }
            return null;
        }

        public StreamlinePropertyInfo GetBySenStayID(string SenStayId)
        {
            var Key = SenStayId.SafeTrim();
            if (SenStayIDToAirBnbName.ContainsKey(Key))
            {
                return SenStayIDToAirBnbName[Key];
            }
            return null;
        }

        public bool Add(StreamlinePropertyInfo PropertyItem)
        {
            var SenStayID = PropertyItem.SenStayID.SafeTrim();
            var AirBnbName = PropertyItem.AirBnbExacName.SafeTrim();

            if (!string.IsNullOrEmpty(SenStayID))
            {
                SenStayIDToAirBnbName[SenStayID] = PropertyItem;
            }

            if (!string.IsNullOrEmpty(AirBnbName))
            {
                AirBnbNameToSenStayID[AirBnbName] = PropertyItem;
            }


            return true;
        }
    }
}