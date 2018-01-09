/// <summary>
/// Streamline price push
/// </summary>

namespace SS_StreamlinePricePush
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Remote;
    using OpenQA.Selenium.Support.UI;
    using SenStaySync;
    using SenStaySync.Data;
    using SenStaySync.PageProcesser.Streamline;
    using SenStaySync.Tools;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Streamline price push class.
    /// </summary>
    public class StreamlinePricePush
    {
        public static void PushPriceToStreamLine(DateTime processStartTime, OperationsJsonLogger<PricePushResult> pricePushLogger,List<Property> streamLineProperties)
        {

            try
            {
                using (RemoteWebDriver driver = SeleniumFactory.GetFirefoxDriver(null))
                {
                    // Login to the streamline system.
                    StreamlineAccount streamLineAccount = new StreamlineAccount();
                    streamLineAccount.Login = Config.I.StreamLineAccountLogin;
                    streamLineAccount.Password = Config.I.StreamLineAccountPassword;

                    N.Note("Trying to login with the user :" + streamLineAccount.Login);
                    var loginResult = StreamlineLogin.ProcessLogin(driver, streamLineAccount);

                    if(loginResult)
                    {
                        N.Note("Login succeeded");

                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                        wait.Until(ExpectedConditions.ElementExists(By.Id("menu")));

                        N.Note("Check whether news dialog has popped up");
                        // Close the dialog which pop's up as soon as you login.
                        StreamlineLogin.CheckNewsDialogPopup(driver);

                        N.Note("Create season group");
                        
                        List<Season> streamLineSeasons = new List<Season>();
                        string groupId = StreamlineSeasonProcesser.CreateSeasonGroup(driver, processStartTime);

                        if (!string.IsNullOrEmpty(groupId) && groupId != "0")
                        {
                            N.Note("Created Group :" + StreamlineSeasonProcesser.GetSeasonGroupName(processStartTime));
                            // Filter the rates based on the valid dates

                            N.Note("Filtering the valid property prices");

                            var validPropertyPrices = streamLineProperties[0].Prices.Where(x => x.SeasonEndDate > DateTime.UtcNow).ToList();

                            N.Note("Create Seasons for the season group");

                            foreach (Price season in validPropertyPrices)
                            {
                                Thread.Sleep(TimeSpan.FromSeconds(1));
                                var seasonId = StreamlineSeasonProcesser.CreateSeason(driver, groupId, season.SeasonStartDate, season.SeasonEndDate, season.MinimumLos);
                                if (!string.IsNullOrEmpty(seasonId) && seasonId != "0")
                                {
                                    N.Note("Created Season : " + StreamlineSeasonProcesser.GetSeasonName(season.SeasonStartDate, season.SeasonEndDate));
                                    streamLineSeasons.Add(new Season {
                                        SeasonStartDate = season.SeasonStartDate,
                                        SeasonEndDate = season.SeasonEndDate,
                                        SeasonId = seasonId
                                    });
                                    //season.SeasonId = seasonId;
                                }
                                else
                                {
                                    pricePushLogger.Log(new PricePushResult(Channel.StreamLine, PricePushLogArea.Season, PricingPushLogType.Error, "Season was not created for the dates starting with " + season.SeasonStartDate.ToShortDateString() + " and ending with " + season.SeasonEndDate.ToShortDateString() + " for the season group" + StreamlineSeasonProcesser.GetSeasonGroupName(processStartTime)));
                                }
                            }
                        }
                        else
                        {
                            pricePushLogger.Log(new PricePushResult(
                                                        Channel.StreamLine,
                                                        PricePushLogArea.SeasonGroup,
                                                        PricingPushLogType.Error,
                                                        "Season group was not created : " + StreamlineSeasonProcesser.GetSeasonGroupName(processStartTime)));
                        }

                        foreach (Property streamLineProperty in streamLineProperties)
                        {
                            N.Note("Check whether property exists : " + streamLineProperty.StreamLineHomeName);
                            // Check whether the property exists in streamline or not.
                            if (StreamlineDailyPrice.CheckWhetherPropertyExists(driver, streamLineProperty.StreamLineHomeId))
                            {
                                    N.Note("Changing the default season group for the property to the new one");
                                    if (StreamlineSeasonProcesser.ChangeSeasonGroup(driver, streamLineProperty.StreamLineHomeId, streamLineProperty.StreamLineHomeName, groupId))
                                    {
                                        N.Note("Default season group changed successfully");

                                        // Map the season id's to the corresponding season in the property
                                        
                                        foreach (Season season in streamLineSeasons)
                                        {
                                            var mappedPropertySeason = streamLineProperty.Prices.FirstOrDefault(p => p.SeasonStartDate == season.SeasonStartDate && p.SeasonEndDate == season.SeasonEndDate);
                                            if (mappedPropertySeason != null)
                                                mappedPropertySeason.SeasonId = season.SeasonId;
                                        }

                                        N.Note("Update season price for the unit");
                                        StreamlineDailyPrice.SetPricesForUnit(driver, Convert.ToInt32(streamLineProperty.StreamLineHomeId), streamLineProperty.Prices, pricePushLogger);
                                    }
                                    else
                                    {
                                        N.Note("Changing the default season group failed");
                                        pricePushLogger.Log(new PricePushResult(
                                                                    Channel.StreamLine, 
                                                                    PricePushLogArea.Property, 
                                                                    PricingPushLogType.Error, 
                                                                    "The process was not able to update the default Season group of the property : " + streamLineProperty.StreamLineHomeName, 
                                                                    Config.I.StreamLineAccountLogin, 
                                                                    streamLineProperty.StreamLineHomeId, 
                                                                    string.Empty,
                                                                    streamLineProperty.StreamLineHomeName,
                                                                    streamLineProperty.PropertyCode,
                                                                    string.Empty, 
                                                                    string.Empty,
                                                                    string.Empty));
                                    }
                            }
                            else
                            {
                                pricePushLogger.Log(new PricePushResult(
                                                                Channel.StreamLine, 
                                                                PricePushLogArea.Property, 
                                                                PricingPushLogType.Error, 
                                                                "The property was not found in Streamline : " + streamLineProperty.StreamLineHomeName, 
                                                                streamLineAccount.Login, 
                                                                streamLineProperty.StreamLineHomeId, 
                                                                string.Empty,
                                                                streamLineProperty.StreamLineHomeName, 
                                                                streamLineProperty.PropertyCode, 
                                                                string.Empty, 
                                                                string.Empty, 
                                                                string.Empty));
                            }
                        }
                    }
                    else
                    {
                        pricePushLogger.Log(new PricePushResult(
                                                    Channel.StreamLine,
                                                    PricePushLogArea.Login,
                                                    PricingPushLogType.Error,
                                                    "Login failed for the user : " + streamLineAccount.Login));
                    }
                }
            }
            catch(Exception ex)
            {
                N.Note(ex.Message);
                pricePushLogger.Log(new PricePushResult(Channel.StreamLine, PricePushLogArea.PriceUpdate, PricingPushLogType.Error, "Pricing push to Streamline failed", ex.Message));
            }
            
        }
    }
}
