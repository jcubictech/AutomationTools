using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenStaySync.Data
{
    public class AirBnbProperty
    {
        public AirBnbProperty()
        { }

        public AirBnbProperty(string propertyCode)
        {
            this.PropertyCode = propertyCode;
        }

        public AirBnbProperty(string propertyCode, int propertyIndex)
        {
            this.PropertyCode = propertyCode;
            this.PropertyIndex = propertyIndex;
        }

        public string PropertyCode { get; set; }
        public string StreamLineHomeName { get; set; }
        public string AirbnbTitle { get; set; }
        public int PropertyIndex { get; set; }
        public string AirbnbId { get; set; }

        public AirBnbAccount AirBnbAccount { get; set; } = new AirBnbAccount();
        public List<AirBnbPrice> Prices { get; set; } = new List<AirBnbPrice>();
    }
}
