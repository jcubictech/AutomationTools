using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SenStaySync
{
    public static class GeneralExtensions
    {

        public static string ToJson(this object Obj, Formatting formattingOptions = Formatting.None)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(Obj, formattingOptions);
        }

        public static int ExtractNumber(this string Value)
        {
            try
            {
                return Convert.ToInt32(Regex.Match(Value, @"\d+").Value);
            }
            catch
            {
                return -1;
            }
        }


        public static string SafeTrim(this string Str)
        {
            return (Str + "").Trim();
        }

        public static void SaveToFileAsJson(this object Obj, string FilePath, Formatting formattingOptions = Formatting.None)
        {
            if (Obj == null) return;
            FileUtils.SaveTextToFile(FilePath, Obj.ToJson(formattingOptions));
        }

        public static string GetSortableStamp(this DateTime dt)
        {
            return dt.ToString("s").Replace(':', '-'); ;
        }


    }


    public static class FileUtils
    {
        public static void SaveTextToFile(string FilePath, string Text)
        {
            File.WriteAllText(FilePath, Text.SafeTrim());
        }


        public static T LoadFromJsonFile<T>(string FilePath)
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(FilePath))
                {
                    String text = sr.ReadToEnd();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text);
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static bool FileExists(string Path)
        {
            return File.Exists(Path);
        }

        public static void BackupFile(string ExistingFilePath)
        {
            File.Copy(ExistingFilePath, ExistingFilePath + "." + DateTime.Now.GetSortableStamp()+ ".backup");
        }



    }
}
