//-----------------------------------------------------------------------
// <copyright file="PricePush.cs" company="Senstay">
//     Senstay 2016.
// </copyright>
//-----------------------------------------------------------------------

/// <summary>
/// AirBnb price push.
/// </summary>
namespace SS_AirBnbPricePush
{
    using OpenQA.Selenium.Remote;
    using SenStaySync;
    using SenStaySync.Data;
    using SenStaySync.Exceptions;
    using SenStaySync.PageProcesser.AirBnb;
    using SenStaySync.Tools;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Price push to Airbnb.
    /// </summary>
    public class PricePush
    {
        /// <summary>
        /// Push price to airbnb.
        /// </summary>
        /// <param name="processStartTime">Process start time.</param>
        /// <param name="pricePushLogger">Price push logger.</param>
        /// <param name="airBnbProperties">List of airbnb properties.</param>
        public static void PushPriceToAirBnb(DateTime processStartTime, OperationsJsonLogger<PricePushResult> pricePushLogger, List<Property> airBnbProperties)
        {
            List<PricePushResult> pricePushLogResults = new List<PricePushResult>();
            try
            {
                if (airBnbProperties != null)
                {
                    // Group the properties by their login email id.
                    var propertiesGroupedByEmail = airBnbProperties.GroupBy(x => x.LoginAccount.Email);

                    if (propertiesGroupedByEmail != null)
                    {
                        foreach (var propertyGroup in propertiesGroupedByEmail)
                        {
                            // Get the property list that needs to be updated.
                            List<Property> properties = propertyGroup.ToList();

                            // Setup the login acccount which should be used to login to the account.
                            var loginAccount = properties[0].LoginAccount;
                            AirBnbAccount account = new AirBnbAccount
                            {
                                Email = loginAccount.Email,
                                Password = loginAccount.Password,
                                ProxyAddress = loginAccount.ProxyAddress,
                                Active = loginAccount.Active,
                                Test = loginAccount.Test
                            };

                            // Set the firefox profile.
                            //var firefoxProfile = SeleniumProxy.GetFirefoxProfileWithProxy(account.ProxyAddress[0]);
                            var chromeOptions = SeleniumProxy.GetChromeOptionsWithProxy(account.ProxyAddress[0]);
                            try
                            {
                                using (RemoteWebDriver driver = SeleniumFactory.GetChromeDriver(chromeOptions))
                                //using (RemoteWebDriver driver = SeleniumFactory.GetFirefoxDriver(firefoxProfile))
                                {
                                    N.Note("Trying to login with the user :" + account.Email);
                               
                                    var signInSucceed = AirBnbLogin.Process(driver, account);

                                    if (signInSucceed.Status == AirBnbLoginStatus.Sucess)
                                    {
                                        ///AirBnbCalendar.ZoomOut(driver);
                                        LogError(pricePushLogResults, PricePushLogArea.Login, PricingPushLogType.Information, "Login succeeded");

                                        foreach (Property property in properties)
                                        {
                                            var validPropertyPrices = property.Prices.Where(x => (x.SeasonEndDate.Date > DateTime.UtcNow.Date));
                                            if (AirBnbCalendar.NavigateToCalendar(driver, property.AirbnbId))
                                            {
                                                ///AirBnbCalendar.ZoomOut(driver);
                                                if (validPropertyPrices.Count() > 0)
                                                {
                                                    string currentMonthYear = string.Empty;
                                                    bool navigateToMonth = true;
                                                    foreach (Price propPrice in validPropertyPrices)
                                                    {
                                                       
                                                        int noOfDays = propPrice.NoOfDaysToBeUpdated();

                                                        if (string.Compare(currentMonthYear, propPrice.SeasonStartDate.Month.ToString() + propPrice.SeasonStartDate.Year.ToString(), true) != 0)
                                                        {
                                                            navigateToMonth = AirBnbCalendar.NavigateToMonth(driver, propPrice.SeasonStartDate.Month, propPrice.SeasonStartDate.Year);

                                                            if (navigateToMonth)
                                                                currentMonthYear = propPrice.SeasonStartDate.Month.ToString() + propPrice.SeasonStartDate.Year.ToString();

                                                            Thread.Sleep(TimeSpan.FromSeconds(2));
                                                        }

                                                        if (navigateToMonth)
                                                        {
                                                            for (int i = 1; i <= noOfDays; i++)
                                                            {
                                                                DateTime startDate = propPrice.SeasonStartDate.AddDays(i);
                                                                if (string.Compare(currentMonthYear, startDate.Month.ToString() + startDate.Year.ToString(), true) != 0)
                                                                {
                                                                    AirBnbCalendar.NavigateToMonth(driver, startDate.Month, startDate.Year);
                                                                    Thread.Sleep(TimeSpan.FromSeconds(2));
                                                                }

                                                                Thread.Sleep(TimeSpan.FromMilliseconds(700));
                                                                N.Note("Updating price for the day :" + propPrice.SeasonStartDate.AddDays(i).ToString("yyyy-MM-dd"));

                                                                try
                                                                {
                                                                    AirBnbCalendar.UpdateUnitPrice(
                                                                                        driver,
                                                                                        propPrice.SeasonStartDate.AddDays(i),
                                                                                        propPrice.PropertyPrice);
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    PricingPushLogType logType = PricingPushLogType.Error;
                                                                    if (ex.GetType() == typeof(PriceUpdateInformation))
                                                                    {
                                                                        logType = PricingPushLogType.Information;
                                                                    }

                                                                    LogPriceUpdationError(pricePushLogResults, PricePushLogArea.PriceUpdate, logType, property, propPrice.SeasonStartDate.AddDays(i), propPrice.PropertyPrice, ex.Message);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            for (int startDay = 1; startDay <= noOfDays; startDay++)
                                                            {
                                                                LogPriceUpdationError(pricePushLogResults, PricePushLogArea.PriceUpdate, PricingPushLogType.Error, property, propPrice.SeasonStartDate.AddDays(startDay), propPrice.PropertyPrice, "Navigating to the month " + propPrice.SeasonStartDate.ToString("MMMM") + " in the calendar failed for the property " + property.AirbnbTitle);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                LogError(pricePushLogResults, PricePushLogArea.Property, PricingPushLogType.Error, property, "Listing Id :" + property.AirbnbId + " for the property " + property.AirbnbTitle + " is wrong");
                                            }

                                            RetryFailedPriceUpdates(driver, pricePushLogResults, property.AirbnbId);
                                        }
                                    }
                                    else
                                    {
                                        LogError(pricePushLogResults, PricePushLogArea.Login, PricingPushLogType.Error, signInSucceed.Message.Replace(System.Environment.NewLine, string.Empty));

                                        try
                                        {
                                            driver.Quit();
                                        }
                                        catch (Exception ex)
                                        {
                                            N.Note("Exception while closing the driver : " + ex.Message);
                                        }
                                    }
                                } 
                            } 
                            catch (Exception ex)
                            {
                                LogError(pricePushLogResults, PricePushLogArea.Property, PricingPushLogType.Error, "Pricing push to AirBnb failed", ex.Message);
                            }
                        }
                    } 
                }
            }
            catch (Exception ex)
            {
                LogError(pricePushLogResults, PricePushLogArea.Property, PricingPushLogType.Error, "Pricing push to AirBnb failed", ex.Message);
            }
            finally
            {
                pricePushLogger.Log(pricePushLogResults);
            }
        }

        /// <summary>
        /// Retry price push for failed updates.
        /// </summary>
        /// <param name="fireFoxDriver">Firefox driver.</param>
        /// <param name="pricePushLogResults">Log list.</param>
        /// <param name="listingId">Listing id.</param>
        public static void RetryFailedPriceUpdates(RemoteWebDriver fireFoxDriver, List<PricePushResult> pricePushLogResults, string listingId)
        {
            var failedPriceUpdates = pricePushLogResults.Where(p => p.Channel == Channel.AirBnb && p.LogArea == PricePushLogArea.PriceUpdate && p.LogType == PricingPushLogType.Error && p.ListingId == listingId);
            if (failedPriceUpdates != null)
            {
                List<PricePushResult> updatesToBeRemoved = new List<PricePushResult>();
                foreach (var failedUpdate in failedPriceUpdates)
                {
                    // Navigate to the property here.
                    AirBnbCalendar.NavigateToCalendar(fireFoxDriver, listingId);

                    // Navigating to the correct month
                    DateTime failedDate = Convert.ToDateTime(failedUpdate.ListingDate);
                    AirBnbCalendar.NavigateToMonth(fireFoxDriver, failedDate.Month, failedDate.Year);
                    
                    Thread.Sleep(TimeSpan.FromSeconds(2));

                    // Try updating the date once again.
                    try
                    {
                        AirBnbCalendar.UpdateUnitPrice(
                                                        fireFoxDriver,
                                                        failedDate,
                                                        Convert.ToDouble(failedUpdate.Price));

                        updatesToBeRemoved.Add(failedUpdate);
                    }
                    catch (Exception ex)
                    {
                        N.Note("Retry :" + ex.Message);
                    }
                }

                foreach (var itemToBeRemoved in updatesToBeRemoved)
                {
                    pricePushLogResults.Remove(itemToBeRemoved);
                }
            }
        }

        /// <summary>
        /// Log data.
        /// </summary>
        /// <param name="pricePushResultsList">List of logs.</param>
        /// <param name="logArea">Log area.</param>
        /// <param name="logType">Log type.</param>
        /// <param name="message">Error message.</param>
        /// <param name="exceptionMessage">Exception message.</param>
        public static void LogError(List<PricePushResult> pricePushResultsList, PricePushLogArea logArea, PricingPushLogType logType, string message, string exceptionMessage = "")
        {
            PricePushResult logResult = new PricePushResult();
            logResult.Channel = Channel.AirBnb;
            logResult.LogArea = logArea;
            logResult.LogType = logType;
            logResult.Message = message;
            logResult.OriginalErrorMessage = exceptionMessage;
            pricePushResultsList.Add(logResult);
        }

        /// <summary>
        /// Log data.
        /// </summary>
        /// <param name="pricePushResultsList">List of logs.</param>
        /// <param name="logArea">Log area.</param>
        /// <param name="logType">Log type.</param>
        /// <param name="property">Property object</param>
        /// <param name="message">Error message.</param>
        public static void LogError(List<PricePushResult> pricePushResultsList, PricePushLogArea logArea, PricingPushLogType logType, Property property, string message)
        {
            PricePushResult logResult = new PricePushResult();
            logResult.Channel = Channel.AirBnb;
            logResult.LogArea = logArea;
            logResult.LogType = logType;
            logResult.Message = message;
            logResult.Login = property.LoginAccount.Email;
            logResult.ListingId = property.AirbnbId;
            logResult.PropertyName = property.AirbnbTitle;
            logResult.PropertyCode = property.PropertyCode;
            logResult.ProxyIP = property.LoginAccount.ProxyAddress[0];
            pricePushResultsList.Add(logResult);
        }

        /// <summary>
        /// Log data.
        /// </summary>
        /// <param name="pricePushResultsList">List of logs.</param>
        /// <param name="logArea">Log area.</param>
        /// <param name="logType">Log type.</param>
        /// <param name="property">Property which is being processed.</param>
        /// <param name="listingDate">Listing date.</param>
        /// <param name="price">Price that needs to be updated.</param>
        /// <param name="message">Error message.</param>
        public static void LogPriceUpdationError(List<PricePushResult> pricePushResultsList, PricePushLogArea logArea, PricingPushLogType logType, Property property, DateTime listingDate, double price, string message)
        {
            PricePushResult logResult = new PricePushResult();
            logResult.Channel = Channel.AirBnb;
            logResult.LogArea = logArea;
            logResult.LogType = logType;
            logResult.Message = message;
            logResult.Login = property.LoginAccount.Email;
            logResult.ListingId = property.AirbnbId;
            logResult.PropertyName = property.AirbnbTitle;
            logResult.PropertyCode = property.PropertyCode;
            logResult.ProxyIP = property.LoginAccount.ProxyAddress[0];
            logResult.ListingDate = listingDate.ToString("yyyy-MM-dd");
            logResult.Price = price.ToString();
            pricePushResultsList.Add(logResult);
        }
    }
}
