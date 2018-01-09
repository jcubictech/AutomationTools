namespace SenStaySync.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using Streamline;

    public static class PropertyMap
    {
        public static List<PropertyInfo> list = new List<PropertyInfo>();

        public static void SetMap(AirBnbRoomMap map, StreamlinePropertyCollection collection)
        {
            var index = collection.CreateIndex();
            list = new List<PropertyInfo>();
            foreach (var airbnbAccount in map.List)
            {
                foreach (var room in airbnbAccount.Rooms)
                {
                    var streamlineInfo = index.GetByAirBnbName(room.RoomTitle);
                    if (streamlineInfo == null)
                    {
                        N.Note("Can't fine property in Streamline data by AirBnb title " + room.RoomTitle);
                        continue;
                    }
                    var propertyInfo = new PropertyInfo
                    {
                        AirbnbAccountEmail = airbnbAccount.AccountEmail,
                        AirbnbID = room.RoomID,
                        AirbnbTitle = room.RoomTitle,
                        SenStayID = streamlineInfo.SenStayID,
                        StreamlineEditID = streamlineInfo.StreamlineEditID
                    };

                    list.Add(propertyInfo);
                }
            }
        }

        public static PropertyInfo GetBySenstayID(string SenstayID)
        {
            try
            {
                var i = from item in list
                    where item.SenStayID == SenstayID
                    select item;
                return i.First();
            }
            catch
            {
                return null;
            }
        }
    }

    public class PropertyInfo
    {
        public string AirbnbAccountEmail;
        public int AirbnbID;
        public string AirbnbTitle;
        public string SenStayID;
        public int StreamlineEditID;
    }
}