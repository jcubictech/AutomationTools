using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SenStaySync.Data;

namespace SenStaySync
{
    using Data.Streamline;

    public static class Indexes
    {
        public static SenStayPropertyIndex SenStay;

        public static void LoadSenStay()
        {
            var StreamlinePropertyList = StreamlinePropertyCollection.Load();
            if (StreamlinePropertyList == null) throw new Exception("Streamline codes are not parsed yet");
            Indexes.SenStay = StreamlinePropertyList.CreateIndex();
        }
    }
}
