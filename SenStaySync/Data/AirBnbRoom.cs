using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SenStaySync;

namespace SenStaySync.Data
{
    public class AirBnbRoom
    {
        public int RoomID;
        public string RoomTitle;
        public string AccountEmail;
    }

    public class AirBnbAccountRooms
    {
        public string AccountEmail;
        public List<AirBnbRoom> Rooms = new List<AirBnbRoom>();
    }

    public class AirBnbRoomMap : ISavable
    {
        public List<AirBnbAccountRooms> List = new List<AirBnbAccountRooms>();

        public void Save()
        {
            this.SaveToFileAsJson(Config.I.AirBnbRoomsMapFile, Newtonsoft.Json.Formatting.Indented);
        }

        public static AirBnbRoomMap Load()
        {
            return LoadUtils.Load<AirBnbRoomMap>(Config.I.AirBnbRoomsMapFile);
        }

        public AirBnbAccountRooms GetAccount(string Email)
        {
            try
            {
                return (from item in List where item.AccountEmail == Email select item).First<AirBnbAccountRooms>();
            }
            catch
            {
                return null;
            }
        }
    }
}
