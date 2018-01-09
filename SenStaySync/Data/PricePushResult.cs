using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenStaySync.Data
{
    public class PricePushResult
    {
        // Type of log whether the section is CSV Parsing, Login to Airbnb, Proxy Error, Already booked error
        public PricePushLogArea LogArea { get; set; }

        public PricingPushLogType LogType { get; set; }

        public string Message { get; set; }

        public string Login { get; set; }

        public string ProxyIP { get; set; }

        public string PropertyName { get; set; }

        public string PropertyCode { get; set; }

        public string ListingId { get; set; }

        public string ListingDate { get; set; }

        public string OriginalErrorMessage { get; set; }

        public string Price { get; set; }

        public Channel Channel { get; set; }

        public PricePushResult()
        {

        }

        public PricePushResult(Channel channel, PricePushLogArea logArea, PricingPushLogType logType, string message)
        {
            this.Channel = channel;
            this.LogArea = logArea;
            this.LogType = logType;
            this.Message = message;
        }

        public PricePushResult(Channel channel, PricePushLogArea logArea, PricingPushLogType logType, string message, string originalMessage)
        {
            this.Channel = channel;
            this.LogArea = logArea;
            this.LogType = logType;
            this.Message = message;
            this.OriginalErrorMessage = originalMessage;
        }

        public PricePushResult(Channel channel, PricePushLogArea logArea, PricingPushLogType logType, string message, string loginUser, string proxyIp)
        {
            this.Channel = channel;
            this.LogArea = logArea;
            this.LogType = logType;
            this.Message = message;
            this.Login = loginUser;
            this.ProxyIP = proxyIp;
        }

        public PricePushResult(PricePushResult pushLog)
        {
            this.Channel = pushLog.Channel;
            this.LogArea = pushLog.LogArea;
            this.LogType = pushLog.LogType;
            this.Message = pushLog.Message;
            this.Login = pushLog.Login;
            this.ProxyIP = pushLog.ProxyIP;
            this.PropertyName = pushLog.PropertyName;
            this.ListingId = pushLog.ListingId;
            this.ListingDate = pushLog.ListingDate;
            this.OriginalErrorMessage = pushLog.OriginalErrorMessage;
            this.PropertyCode = pushLog.PropertyCode;
            this.Price = pushLog.Price;
        }

        [JsonConstructor]
        public PricePushResult(Channel channel,PricePushLogArea logArea, PricingPushLogType logType, string message, string loginUser, string listingId, string listingDate, string propertyName, string propertyCode, string price, string originalErrorMessage, string proxyIp)
        {
            this.Channel = channel;
            this.LogArea = logArea;
            this.LogType = logType;
            this.Message = message;
            this.Login = loginUser;
            this.ProxyIP = proxyIp;
            this.PropertyName = propertyName;
            this.ListingId = listingId;
            this.ListingDate = listingDate;
            this.OriginalErrorMessage = OriginalErrorMessage;
            this.PropertyCode = propertyCode;
            this.Price = price;
        }

    }

    public enum PricePushLogArea
    {
        ParsingCSV = 1,
        CSVFileNotFound = 2,
        Login = 3,
        Property = 4,
        PriceUpdate = 5,

        // Streamline Specific
        Season = 6,
        SeasonGroup = 7,
        ChangeSeasonGroup = 8,
        ProcessStarted = 9
    }

    public enum PricingPushLogType
    {
        Error = 1,
        Information = 2,
        Warning = 3
    }

    public enum Channel
    {
        AirBnb = 1,
        StreamLine = 2,
        Common = 3
    }
}
