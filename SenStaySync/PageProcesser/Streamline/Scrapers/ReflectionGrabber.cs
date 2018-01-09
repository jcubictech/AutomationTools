namespace SenStaySync.PageProcesser.Streamline.Scrapers
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;
    using System.Threading.Tasks;
    using Data;
    using OpenQA.Selenium;

    public class ReflectionGrabber
    {
        public void GrabPropertiesByNameOrId<T>(IWebDriver driver, T item)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var propertyByAttribute = properties.Where(x =>
            {
                var formFieldAttribute = x.GetCustomAttributes<FormFieldAttribute>().SingleOrDefault();
                return formFieldAttribute != null && (!string.IsNullOrEmpty(formFieldAttribute.Name) || !string.IsNullOrEmpty(formFieldAttribute.Id));
            }).ToList();

            foreach (var property in propertyByAttribute)
            {
                var fieldAttribute = property.GetCustomAttribute<FormFieldAttribute>();
                try
                {
                    if (!string.IsNullOrEmpty(fieldAttribute.Name))
                    {
                        var element = driver.FindElement(By.Name(fieldAttribute.Name));
                        if (element != null)
                        {
                            property.SetValue(item, element.GetFieldValue(fieldAttribute.Type));
                        }
                    }
                }
                catch (NoSuchElementException)
                {
                }

                try
                {
                    if (!string.IsNullOrEmpty(fieldAttribute.Id))
                    {
                        var element = driver.FindElement(By.Id(fieldAttribute.Id));
                        if (element != null)
                        {
                            property.SetValue(item, element.GetFieldValue(fieldAttribute.Type));
                        }
                    }
                }
                catch (NoSuchElementException)
                {
                }
            }
        }

        public T TwoColumnTableGrab<T>(IWebElement table, bool leftForLabel = true, T item = null) where T : class
        {
            var rows = table.FindElements(By.XPath("./tbody/tr")).ToList();
            var result = item ?? Activator.CreateInstance<T>();

            Parallel.ForEach(rows, (row) =>
            {
                var cells = row.FindElements(By.XPath("./td")).ToList();

                if (cells.Count <= 1)
                    return;

                var propertyName = leftForLabel ? cells[0].Text.Trim('[', '?', ']', ':').Trim() : cells[1].Text.Trim('[', '?', ']', ':').Trim();
                var cellValue = leftForLabel ? cells[1] : cells[0];
                var cacheKey = typeof(T).FullName + propertyName;
                var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

                var propertyByAttribute = (System.Reflection.PropertyInfo) MemoryCache.Default.Get(cacheKey) ??
                properties.SingleOrDefault(x =>
                {
                    var formFieldAttribute = x.GetCustomAttributes<FormFieldAttribute>().SingleOrDefault();
                    return formFieldAttribute != null && formFieldAttribute.Field == propertyName;
                });

                if (propertyByAttribute != null)
                {
                    if (!MemoryCache.Default.Contains(cacheKey))
                        MemoryCache.Default.Add(cacheKey, propertyByAttribute, DateTimeOffset.MaxValue);

                    var fieldAttribute = propertyByAttribute.GetCustomAttribute<FormFieldAttribute>();
                    var value = cellValue.GetChildFieldValue(fieldAttribute.Type);

                    var s = value as string;

                    if (s != null && propertyByAttribute.PropertyType != typeof (string))
                    {
                        propertyByAttribute.SetValue(result, s.ConvertTo(propertyByAttribute.PropertyType));
                    }
                    else
                    {
                        propertyByAttribute.SetValue(result, value);
                    }
                    return;
                }

                var propertyByName = properties.SingleOrDefault(x => x.Name == propertyName);

                if (propertyByName != null)
                {
                    propertyByName.SetValue(result, cellValue.Text);
                }
            });

            return result;
        }
    }
}