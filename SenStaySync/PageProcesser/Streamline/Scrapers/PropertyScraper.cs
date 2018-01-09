namespace SenStaySync.PageProcesser.Streamline.Scrapers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Data;
    using Data.Streamline;
    using OpenQA.Selenium;

    public class PropertyScraper
    {
        private readonly IWebDriver _driver;

        public PropertyScraper(IWebDriver driver)
        {
            _driver = driver;
        }

        public StreamlinePropertyInfo Scrap(StreamlinePropertyInfo item, bool grabAdditionalInfo)
        {
            _driver.Navigate().GoToUrl(@"https://www.resortpro.net/new/admin/edit_home.html?home_id=" + item.StreamlineEditID + @"&activetab=0&need_update_parent=0");
            _driver.JustWait(5);

            var nameElement = _driver.FindElement(By.Id("name"));
            var streamlineName = nameElement.GetAttribute("value");

            var airBnbNameElement = _driver.FindElement(By.Id("airbnb_home_name"));
            var airBnbName = airBnbNameElement.GetAttribute("value");

            item.StreamlinePropertyName = streamlineName;
            item.AirBnbExacName = airBnbName;
            item.SenStayID = streamlineName.ExtractSenStayID();

            if (!grabAdditionalInfo)
            {
                Console.WriteLine("Processed # " + item.SenStayID + " " + item.AirBnbExacName);
                return item;
            }

            item.HouseDetails = GrabHouseDetails();
            item.CustomAmenities = GrabCustomAmenities();
            item.HaVrboAmenities = GrabHaVrboAmenities();
            item.TaxesAndFees = GrabTaxesAndFees();
            item.OwnerInformation = GrabOwnerInformation();

            GrabGalleryAsync(item.StreamlineEditID.ToString()).Wait();

            Console.WriteLine("Processed # " + item.SenStayID + " " + item.AirBnbExacName);
            return item;
        }

        private PropertyOwnerInformation GrabOwnerInformation()
        {
            var ownerInformation = new PropertyOwnerInformation();
            var grabber = new ReflectionGrabber();

            var ownerDetailsLink = _driver.FindElement(By.LinkText("Owner Details"));
            ownerDetailsLink.Click();

            var formTables = _driver
                .FindElement(By.Id("HomeModifyOwnerDetails_form"))
                .FindElements(By.XPath("./table"));

            var ownerDetailsCell = formTables[0]
                .FindElement(By.XPath("./tbody/tr"))
                .FindElements(By.XPath("./td"))[1];

            var ownerAccountDetailsCell =
                formTables[1]
                    .FindElement(By.XPath("./tbody/tr"))
                    .FindElements(By.XPath("./td"))[1];

            var mastercells = new List<IWebElement>
            {
                ownerAccountDetailsCell,
                ownerDetailsCell
            };

            foreach (var mastercell in mastercells)
            {
                var tables = mastercell.FindElements(By.CssSelector("table")).ToList();
                foreach (var table in tables)
                {
                    grabber.TwoColumnTableGrab(table, true, ownerInformation);
                }
            }

            return ownerInformation;
        }

        private HouseDetails GrabHouseDetails()
        {
            var houseDetails = new HouseDetails();
            var grabber = new ReflectionGrabber();

            var formtable = _driver
                .FindElement(By.TagName("form"))
                .FindElement(By.TagName("table"));

            var tableBody = formtable
                .FindElement(By.TagName("tbody"));

            var mastercells = tableBody
                .FindElement(By.TagName("tr"))
                .FindElements(By.XPath("./td"))
                .ToList();

            foreach (var mastercell in mastercells)
            {
                var tables = mastercell.FindElements(By.CssSelector("table")).ToList();
                foreach (var table in tables)
                {
                    grabber.TwoColumnTableGrab(table, true, houseDetails);
                }
            }

            grabber.GrabPropertiesByNameOrId(_driver, houseDetails);

            var descriptionLink = _driver.FindElement(By.LinkText("Desc."));
            descriptionLink.Click();

            _driver.JustWait(5);

            grabber.GrabPropertiesByNameOrId(_driver, houseDetails);

            return houseDetails;
        }

        private Dictionary<string, bool> GrabTaxesAndFees()
        {
            var galleryLink = _driver.FindElement(By.LinkText("Taxes&Fees"));
            galleryLink.Click();

            _driver.JustWait(5);

            var table = _driver.FindElement(By.Id("home_taxes_form")).FindElement(By.TagName("table"));
            var rows = table.FindElements(By.XPath("./tbody/tr")).ToList();
            var result = new ConcurrentDictionary<string, bool>();

            Parallel.ForEach(rows, row =>
            {
                var cells = row.FindElements(By.XPath("./td")).ToList();

                if (cells.Count <= 1)
                    return;

                var propertyName = cells[1].Text.Trim('[', '?', ']', ':').Trim();
                var cellValue = cells[0];
                result.TryAdd(propertyName, (bool) cellValue.GetChildFieldValue(FormFieldType.Checkbox));
            });

            return result.ToDictionary(x => x.Key, x => x.Value);
        }

        private Dictionary<string, bool> GrabCustomAmenities()
        {
            var galleryLink = _driver.FindElement(By.LinkText("Custom Amenities"));
            galleryLink.Click();

            _driver.JustWait(5);

            var table = _driver.FindElement(By.Id("amenities_form")).FindElement(By.TagName("table"));
            var rows = table.FindElements(By.XPath("./tbody/tr")).ToList();
            var result = new ConcurrentDictionary<string, bool>();

            Parallel.ForEach(rows, row =>
            {
                var cells = row.FindElements(By.XPath("./td")).ToList();

                if (cells.Count <= 1)
                    return;

                var propertyName = cells[1].Text.Trim('[', '?', ']', ':').Trim();
                var cellValue = cells[0];
                result.TryAdd(propertyName, (bool) cellValue.GetChildFieldValue(FormFieldType.Checkbox));
            });

            return result.ToDictionary(x => x.Key, x => x.Value);
        }

        private Dictionary<string, bool> GrabHaVrboAmenities()
        {
            try
            {
                var galleryLink = _driver.FindElement(By.LinkText("HA/VRBO Amenities"));
                galleryLink.Click();

                _driver.JustWait(5);

                var table = _driver.FindElement(By.Id("homeaway_details_form")).FindElement(By.TagName("table"));
                var rows = table.FindElements(By.XPath("./tbody/tr")).ToList();
                var result = new ConcurrentDictionary<string, bool>();

                Parallel.ForEach(rows, row =>
                {
                    var cells = row.FindElements(By.XPath("./td")).ToList();

                    if (cells.Count <= 1)
                        return;

                    var propertyName = cells[1].Text.Trim('[', '?', ']', ':').Trim();
                    var cellValue = cells[0];
                    result.TryAdd(propertyName, (bool)cellValue.GetChildFieldValue(FormFieldType.Checkbox));
                });

                return result.ToDictionary(x => x.Key, x => x.Value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task GrabGalleryAsync(string streamlineEditId)
        {
            var galleryLink = _driver.FindElement(By.LinkText("Gallery"));
            galleryLink.Click();

            _driver.JustWait(5);

            var images = _driver.FindElements(By.CssSelector(".galleryimage"));
            var imageLinks = images.Select(image => image.GetAttribute("src")).ToList();
            var i = 0;

            using (var webClient = new WebClient())
            {
                foreach (var thumbLink in imageLinks)
                {
                    i++;
                    var fullSizeLink = thumbLink.Replace("thumbnail", "image");
                    await
                        webClient.DownloadFileTaskAsync(new Uri(fullSizeLink),
                            Path.Combine(Config.I.ImagesFolder, streamlineEditId + "_" + i + ".jpg"));
                }
            }
        }


    }
}