using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenStaySync.PageProcesser.AirBnb
{
    public class AirBnbPricePushResult
    {
        // Type of log whether the section is CSV Parsing, Login to Airbnb, Proxy Error, Already booked error
        public PricePushLogType Type { get; set; }
        
        public string Message { get; set; }

        public string Login { get; set; }

        public string ProxyIP { get; set; }

        public string PropertyName { get; set; }

        public string ListingId { get; set; }

        public string ListingDate { get; set; }

        public AirBnbPricePushResult(PricePushLogType type, string message)
        {
            this.Type = type;
            this.Message = message;
        }

        public AirBnbPricePushResult(PricePushLogType type, string message, string loginUser, string proxyIp)
        {
            this.Type = type;
            this.Message = message;
            this.Login = loginUser;
            this.ProxyIP = proxyIp;
        }

        public AirBnbPricePushResult(PricePushLogType type, string message, string loginUser, string listingId, string listingDate)
        {
            this.Type = type;
            this.Message = message;
            this.Login = loginUser;
            this.ListingId = listingId;
            this.ListingDate = listingDate;
        }

        [JsonConstructor]
        public AirBnbPricePushResult(PricePushLogType type, string message, string loginUser, string proxyIp, string propertyName, string listingId, string listingDate)
        {
            this.Type = type;
            this.Message = message;
            this.Login = loginUser;
            this.ProxyIP = proxyIp;
            this.PropertyName = propertyName;
            this.ListingId = listingId;
            this.ListingDate = listingDate;
        }
        
    }

    public enum PricePushLogType
    {
        ParsingCSV = 1,
        CSVFileNotFound = 2,
        Login = 3,
        Property = 4, 
        PriceUpdate = 5
    }
}
