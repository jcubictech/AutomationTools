using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SenStaySync.Tools
{
    public static class CSVReader
    {
        public static List<Tuple<string, string>> Read2columnCSV(string path)
        {
            var reader = new StreamReader(File.OpenRead(path));
            var data = new List<Tuple<string, string>>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                data.Add(Tuple.Create<string, string>(values[0], values[1]));
            }
            return data;
        }

        public static List<SourcePriiceItem> Read4columnCSV(string path)
        {
            var reader = new StreamReader(File.OpenRead(path));
            var data = new List<SourcePriiceItem>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                data.Add(new SourcePriiceItem() {
                    Date = (values[0] + "").Trim(),
                    Unit = (values[1] + "").Trim(),
                    Prce = (values[2] + "").Trim(),
                    TimeStamp = (values[3] + "").Trim()
                });
            }
            return data;
        }
    }

    public class SourcePriiceItem
    {
        public string Date;
        public string Unit;
        public string Prce;
        public string TimeStamp;
    }
}
