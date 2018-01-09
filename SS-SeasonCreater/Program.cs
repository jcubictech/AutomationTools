using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SenStaySync;

namespace SS_SeasonCreater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Daily season generator 2016-03-29");
            
            Config.I.UseProxy = false;
            //Config.UseProxy = true;
            var Account = new SenStaySync.Data.StreamlineAccount()
            {
                Login = "devtest1",
                Password = "DevTest11"
            };

            
            var Driver = SenStaySync.SeleniumFactory.GetFirefoxDriver();
            //try {

            SenStaySync.PageProcesser.Streamline.StreamlineLogin.Process(Driver, Account, false);
            //SenStaySync.PageProcesser.Streamline.StreamlineSeasonProcesser.CreateSeason(Driver, 2, 29);
            //SenStaySync.PageProcesser.Streamline.StreamlineSeasonProcesser.CreateSeason(Driver, 3, 8);
            //SenStaySync.PageProcesser.Streamline.StreamlineSeasonProcesser.CreateSeason(Driver, 4, 9);

            //SeasonGenerator.GenerateDailySeasonsForYear(Driver, "20250");
            //SeasonGenerator.GenerateDailySeasonsForYear(Driver, "20826");
            //SeasonGenerator.GenerateDailySeasonsForYear(Driver, "21701");
            SeasonGenerator.GenerateDailySeasonsForYear(Driver, "21713");
            


            //} catch { Driver.Quit(); }

            Driver.Quit();

            //SeasonGenerator.GenerateDailySeasonsForYear(2016);
        }


    }

    public static class SeasonGenerator
    {
        public static void GenerateDailySeasonsForYear(IWebDriver driver, string GroupID)
        {
            //var January1 = new DateTime(Year, 1, 1);
            //var January1 = new DateTime(2017, 1, 1);
            var January1 = new DateTime(2016, 5, 26);
            var lastDay = new DateTime(2017, 3, 22);
            var List = new List<DailySeasonItem>();
            for (int i = 0; January1 < lastDay; i++)
            {
                var CurrentDaySeason = new DateTime(January1.Ticks).AddDays(i);
                try
                {
                    SenStaySync.PageProcesser.Streamline.StreamlineSeasonProcesser.CreateSeason(driver, CurrentDaySeason.Year, CurrentDaySeason.Month, CurrentDaySeason.Day, GroupID);
                }
                catch
                {
                    try
                    {
                        SenStaySync.PageProcesser.Streamline.StreamlineSeasonProcesser.CreateSeason(driver, CurrentDaySeason.Year, CurrentDaySeason.Month, CurrentDaySeason.Day, GroupID);
                    }
                    catch
                    { }
                }
                    /*
                    var Item = new DailySeasonItem()
                    {
                        DayDate = CurrentDaySeason,
                        DayDateFormated = CurrentDaySeason.ToString("MM/dd/yyyy"),
                        NumberOfTheDay = i + 1,
                        SeasonName = CurrentDaySeason.ToString("yyyy-MM-dd"),
                        SeasonDescription = CurrentDaySeason.ToString("MMMM dd yyyy")
                    };
                    List.Add(Item);
                    //*/
                }
            //return List;
        }
    }

    public class DailySeasonItem
    {
        public int NumberOfTheDay = 1;
        public DateTime DayDate;
        public string SeasonName;
        public string SeasonDescription;

        /// <summary>
        /// Both Strat and End dates are the same
        /// </summary>
        public string DayDateFormated = "01/02/2016";
    }

    public class SeasonSeleniumProcesser
    {

    }

    public class DailySeasonMap
    {
        public static void GetSeasonByDay()
        {

        }
    }

    public class DailySeason
    {
        public string ID;
        public DateTime Date;

        public bool IsDate (DateTime date)
        {
            return Date.Month == date.Month && Date.Day == date.Day;
        }
    }
}


/*

<ul class="yui-nav">
<li title="active" class="selected"><a href="#tab1"><em>General</em></a></li>
<li title=""><a href="#tab2"><em>Description</em></a></li>
<li title=""><a href="#tab4"><em>Define Periods</em></a></li>
</ul>


General:

Group <select style="width:300px" size="1" id="group_id" name="group_id"> = <option value="20096">365 days
Name <input type="text" maxlength="64" value="" style="width:99%" id="name" name="name">
Nightly Minimum         <select style="width:50px;" size="1" id="narrow_defined_days" name="narrow_defined_days"> = <option value="1">1
Use Daily Pricing       <input type="checkbox" value="1" name="use_pricing_model[]"></td></tr>
Use Weekly Pricing      <input type="checkbox" value="2" name="use_pricing_model[]"></td></tr>
Use Monthly Pricing     <input type="checkbox" value="3" name="use_pricing_model[]"></td></tr>


Description:
<textarea style="width:99%;" rows="12" name="description"></textarea>

Define Periods:


<tr>
<td align="left"><input type="text" style="width:100%" value="" id="new_name" name="new_name"></td> = 2 January
<td align="center"><input type="text" id="date_start_new" name="date_start_new"> = 01/02/2016 using JS
<td align="center"><input type="text" id="date_end_new" name="date_end_new"> = 01/02/2016  using JS
<td align="center">&nbsp;</td>
</tr>

//*/
