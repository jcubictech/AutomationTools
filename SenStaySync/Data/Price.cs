using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenStaySync.Data
{
    public class Price
    {

        public double PropertyPrice { get; set; }
        public DateTime SeasonStartDate { get; set; }
        public DateTime SeasonEndDate { get; set; }
        public int MinimumLos { get; set; }
        public DateTime DateStamp { get; set; }
        public string SeasonId { get; set; }

        public bool isValidDate()
        {
            // Check whether Season start date is greater than current date.
            return (SeasonStartDate > DateTime.Now);
        }

        public int NoOfDaysToBeUpdated()
        {
            TimeSpan ts = SeasonEndDate - SeasonStartDate;
            return ts.Days;
        }
    }
}
