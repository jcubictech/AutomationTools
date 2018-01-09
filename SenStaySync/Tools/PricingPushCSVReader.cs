using System;
using System.Collections.Generic;
using SenStaySync.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SenStaySync.PageProcesser.AirBnb;
using System.Globalization;

namespace SenStaySync.Tools
{
    public class PricingPushCSVReader
    {
        public static List<Property> ReadPricingData(OperationsJsonLogger<PricePushResult> logger)
        {
            List<Property> properties = null;
            IFormatProvider culture = new System.Globalization.CultureInfo("en-US", true);
            int propertCodeIndex = 0;
            int seasonStartIndex = 0;
            int seasonEndIndex = 0;
            int minLosStartIndex = 0;
            int dateStampIndex = 0;
            int priceRowIndex = 7;

            try
            {
                //var lines = File.ReadAllLines(Config.I.AirBnbPricingPushFile);
                var lines = WriteSafeReadAllLines(Config.I.PricingPushFile);
                if (lines.Length > 0)
                {
                    properties = new List<Property>();
                }

                for (int index = 0; index < lines.Length; index++)
                {
                    // Getting the list of properties.
                    if (index == propertCodeIndex)
                    {
                        string[] propertyArray = lines[index].Split(',');
                        int propertyCellIndex = Array.IndexOf(propertyArray, PricingPushCsv.PropertyCode);
                        int propertyCodesIndex = propertyCellIndex + 1;
                        while (propertyCodesIndex < propertyArray.Length)
                        {
                            if (!string.IsNullOrEmpty(propertyArray[propertyCodesIndex]))
                            {
                                Property property = new Property(propertyArray[propertyCodesIndex], propertyCodesIndex);
                                properties.Add(property);
                            }
                            propertyCodesIndex++;
                        }
                    }

                    // Better than checking whether the index is greater than 1 would be to check if its greater than or equal to 0 
                    // The whole purpose is to check whether the line exists or not.
                    // Getting the stream line home names.
                    if (Array.IndexOf(lines[index].Split(','), PricingPushCsv.StreamLineHomeName) >= 0)
                    {
                        string[] streamLineHomeNames = lines[index].Split(',');
                        foreach (Property property in properties)
                        {
                            property.StreamLineHomeName = streamLineHomeNames[property.PropertyIndex].Trim();
                        }
                    }

                    // Parsing and reading the stream line home id's
                    if(Array.IndexOf(lines[index].Split(','), PricingPushCsv.StreamLineHomeId) >= 0)
                    {
                        string[] streamLineHomeIds = lines[index].Split(',');
                        foreach (Property property in properties)
                        {
                            property.StreamLineHomeId = streamLineHomeIds[property.PropertyIndex].Trim();
                        }
                    }

                    //Getting the airbnb accounts.
                    if (Array.IndexOf(lines[index].Split(','), PricingPushCsv.AirbnbAccount) >= 0)
                    {
                        string[] airBnbAccountNames = lines[index].Split(',');
                        foreach (Property property in properties)
                        {
                            //property.AirbnbAccount = airBnbAccountNames[property.PropertyIndex].Trim();
                            property.LoginAccount.Email = airBnbAccountNames[property.PropertyIndex].Trim();

                        }
                    }

                    //Getting the airbnb passswords.
                    if (Array.IndexOf(lines[index].Split(','), PricingPushCsv.AirbnbPassword) >= 0)
                    {
                        string[] airBnbAccountPasswords = lines[index].Split(',');
                        foreach (Property property in properties)
                        { 
                            property.LoginAccount.Password = airBnbAccountPasswords[property.PropertyIndex].Trim();
                        }
                    }

                    //Getting the ProxyIP for the accounts.
                    if (Array.IndexOf(lines[index].Split(','), PricingPushCsv.ProxyIp) >= 0)
                    {
                        string[] proxyIps = lines[index].Split(',');
                        foreach (Property property in properties)
                        {
                            property.LoginAccount.ProxyAddress = new List<string> { proxyIps[property.PropertyIndex].Trim() };
                            //property.ProxyIP = proxyIps[property.PropertyIndex].Trim();
                        }
                    }

                    //Getting the Airbnb title for the properties.
                    if (Array.IndexOf(lines[index].Split(','), PricingPushCsv.AirbnbTitle) >= 0)
                    {
                        string[] airBnbTitles = lines[index].Split(',');
                        foreach (Property property in properties)
                        {
                            property.AirbnbTitle = airBnbTitles[property.PropertyIndex].Trim();
                        }
                    }

                    if (Array.IndexOf(lines[index].Split(','), PricingPushCsv.AirbnbListingId) >= 0)
                    {
                        string[] airBnbListingIds = lines[index].Split(',');
                        foreach (Property property in properties)
                        {
                            property.AirbnbId = airBnbListingIds[property.PropertyIndex].Trim();
                        }
                    }


                    //Getting the price index for the properties.
                    if (Array.IndexOf(lines[index].Split(','), PricingPushCsv.DateStamp) >= 0)
                    {
                        string[] propertyPriceHeadings = lines[index].Split(',');
                        seasonStartIndex = Array.IndexOf(propertyPriceHeadings, PricingPushCsv.SeasonStartDate);
                        seasonEndIndex = Array.IndexOf(propertyPriceHeadings, PricingPushCsv.SeasonEndDate);
                        dateStampIndex = Array.IndexOf(propertyPriceHeadings, PricingPushCsv.DateStamp);
                        minLosStartIndex = Array.IndexOf(propertyPriceHeadings, PricingPushCsv.MinimumLOS);
                        priceRowIndex = index + 1;
                    }

                    // Getting and populating the object with prices. 
                    if (index >= priceRowIndex)
                    {
                        string[] prices = lines[index].Split(',');
                        if (string.IsNullOrEmpty(prices[0]))
                        {
                            continue;
                        }

                        try
                        {
                            string dayPrice = string.Empty;
                            foreach (Property property in properties)
                            {
                                Price propertyPrice = new Price();
                                propertyPrice.SeasonStartDate = DateTime.Parse(prices[seasonStartIndex].Trim(), CultureInfo.InvariantCulture);
                                propertyPrice.SeasonEndDate = DateTime.Parse(prices[seasonEndIndex].Trim(), CultureInfo.InvariantCulture);
                                dayPrice = prices[property.PropertyIndex].Trim(new char[] { '$' });
                                if (!string.IsNullOrEmpty(dayPrice))
                                {
                                    propertyPrice.PropertyPrice = Convert.ToDouble(dayPrice);
                                    //Convert.ToDouble(prices[property.PropertyIndex].Trim(new char[] { '$' }));
                                }
                                propertyPrice.MinimumLos = Convert.ToInt32(prices[minLosStartIndex].Trim());
                                propertyPrice.DateStamp = DateTime.Parse(prices[dateStampIndex].Trim(), CultureInfo.InvariantCulture);
                                property.Prices.Add(propertyPrice);
                            }
                        }
                        catch(Exception ex)
                        {
                            logger.Log(new PricePushResult(Channel.Common, PricePushLogArea.ParsingCSV, PricingPushLogType.Error, ex.Message + ", Season Start Date :"+ prices[seasonStartIndex].Trim()+", Season End Date :"+ prices[seasonEndIndex].Trim()));
                        }
                    }

                }

                return properties;
            }
            catch (FileNotFoundException ex)
            {
                logger.Log(new PricePushResult(Channel.Common, PricePushLogArea.CSVFileNotFound, PricingPushLogType.Error, ex.Message));
            }
            catch (Exception ex)
            {
                logger.Log(new PricePushResult(Channel.Common, PricePushLogArea.ParsingCSV, PricingPushLogType.Error, ex.Message));
            }
            return properties;
        }


        /// <summary>
        /// Safely read all the lines, without any process being read by another process error.
        /// </summary>
        /// <param name="path">Path of the file to be read from</param>
        /// <returns>Array of string values read from the csv file.</returns>
        public static string[] WriteSafeReadAllLines(String path)
        {
            using (var csv = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(csv))
            {
                List<string> file = new List<string>();
                while (!sr.EndOfStream)
                {
                    file.Add(sr.ReadLine());
                }

                return file.ToArray();
            }
        }

        /// <summary>
        /// Log which all properties will be updated and by whom
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="pricePushLogger"></param>
        public static void LogPropertiesForAirBnb(List<Property> properties, OperationsJsonLogger<PricePushResult> pricePushLogger)
        {
            List<PricePushResult> pushResults = new List<PricePushResult>();
            foreach (Property property in properties)
            {
                pushResults.Add(new PricePushResult(
                     Channel.AirBnb,
                     PricePushLogArea.ParsingCSV,
                     PricingPushLogType.Information, 
                     string.Empty,
                     property.LoginAccount.Email, 
                     property.LoginAccount.ProxyAddress[0], 
                     property.AirbnbTitle, 
                     property.AirbnbId,
                     string.Empty,
                     string.Empty,
                     string.Empty, 
                     string.Empty
                ));
            }
            pricePushLogger.Log(pushResults);
        }

        /// <summary>
        /// Log which all properties will be updated and by whom
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="pricePushLogger"></param>
        public static void LogPropertiesForStreamLine(List<Property> properties, OperationsJsonLogger<PricePushResult> pricePushLogger)
        {
            List<PricePushResult> pushResults = new List<PricePushResult>();
            foreach (Property property in properties)
            {
                pushResults.Add(new PricePushResult(
                     Channel.StreamLine,
                     PricePushLogArea.ParsingCSV,
                     PricingPushLogType.Information,
                     string.Empty,
                     Config.I.StreamLineAccountLogin,
                     string.Empty,
                     property.StreamLineHomeName,
                     property.StreamLineHomeId,
                     string.Empty,
                     string.Empty,
                     string.Empty,
                     string.Empty
                ));
            }
            pricePushLogger.Log(pushResults);
        }
    }

    public static class PricingPushCsv
    {
        public const string PropertyCode = "Property Code";
        public const string StreamLineHomeName = "Streamline Home Name";
        public const string StreamLineHomeId = "Streamline Home Id";
        public const string AirbnbAccount = "Airbnb Account";
        public const string AirbnbPassword = "Account Password";
        public const string AirbnbListingId = "Airbnb Id";
        public const string ProxyIp = "Proxy IP";
        public const string AirbnbTitle = "Airbnb Title";
        public const string SeasonStartDate = "Season Start";
        public const string SeasonEndDate = "Season End";
        public const string MinimumLOS = "Minimum LOS";
        public const string PropertyPrice = "Prices";
        public const string DateStamp = "Date Stamp";
    }

}



