using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenStaySync.Data
{
    public class Property
    {
        public Property()
        { }

        public Property(string propertyCode)
        {
            this.PropertyCode = propertyCode;
        }

        public Property(string propertyCode, int propertyIndex)
        {
            this.PropertyCode = propertyCode;
            this.PropertyIndex = propertyIndex;
        }

        public string PropertyCode { get; set; }
        public string StreamLineHomeName { get; set; }
        public string StreamLineHomeId { get; set; }
        public string AirbnbTitle { get; set; }
        public int PropertyIndex { get; set; }
        public string AirbnbId { get; set; }

        public LoginAccount LoginAccount { get; set; } = new LoginAccount();
        public List<Price> Prices { get; set; } = new List<Price>();
    }
}
