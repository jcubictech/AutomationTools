namespace SenStaySync.Tools
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Data;
    using PageProcesser.AirBnb;

    public class OperationsJsonLogger<T>
    {
        protected string CurrentPath { get; set; }
        public string OriginalPath { get; set; }

        public OperationsJsonLogger(string path)
        {
            Temp.TouchDirectory(Path.GetDirectoryName(path));
            OriginalPath = path;
            CurrentPath = GetPath(path, DateTime.UtcNow);
        }

        public OperationsJsonLogger(string path, DateTime processKickStartedTime)
        {
            Temp.TouchDirectory(Path.GetDirectoryName(path));
            OriginalPath = path;
            CurrentPath = GetPathWithDateTimeAppended(path, processKickStartedTime);
        }

        protected void CreateFile()
        {
            if (!File.Exists(CurrentPath))
                File.Create(CurrentPath).Close();
        }

        public void Log(T item)
        {
            CreateFile();

            var list = FileUtils.LoadFromJsonFile<List<T>>(CurrentPath) ?? new List<T>();

            list.Add(item);
            list.SaveToFileAsJson(CurrentPath);
        }

        public void Log(List<T> items)
        {
            CreateFile();

            var list = FileUtils.LoadFromJsonFile<List<T>>(CurrentPath) ?? new List<T>();

            list.AddRange(items);
            list.SaveToFileAsJson(CurrentPath);
        }

        public List<T> GetLogRecords(DateTime? logDate = null)
        {
            var path = GetPath(OriginalPath, logDate ?? DateTime.UtcNow);
            var logRecords = FileUtils.LoadFromJsonFile<List<T>>(path) ?? new List<T>();
            return logRecords;
        }

        public List<T> GetLogRecordsForExactDateTime(DateTime? logDate = null)
        {
            var path = GetPathWithDateTimeAppended(OriginalPath, logDate ?? DateTime.UtcNow);
            var logRecords = FileUtils.LoadFromJsonFile<List<T>>(path) ?? new List<T>();
            return logRecords;
        }

        public void Clear()
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(CurrentPath));
            var currentFileName = Path.GetFileName(CurrentPath);
            var originalFileName = Path.GetFileName(OriginalPath);

            var mask = ReplaceLastOccurrence(originalFileName, ".", "*.");

            foreach (var file in dir.EnumerateFiles(mask).Where(file => file.Name != currentFileName))
            {
                file.Delete();
            }
        }

        private string GetPath(string path, DateTime date)
        {
            var trailDate = "-" + (date).ToString("yyyy-M-d") + ".";
            return ReplaceLastOccurrence(path, ".", trailDate);
        }

        private string GetPathWithDateTimeAppended(string path, DateTime date)
        {
            var trailDate = "-" + (date).ToString("yyyy-M-d-HH-mm") + ".";
            return ReplaceLastOccurrence(path, ".", trailDate);
        }

        private string ReplaceLastOccurrence(string source, string find, string replace)
        {
            var place = source.LastIndexOf(find, StringComparison.Ordinal);

            if (place == -1)
                return source;

            var result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }
    }

    public static class OperationsJsonLoggerExtensions
    {
        public static LoginStatistics GetStatistics(this OperationsJsonLogger<LoginAttempt> logger, DateTime date)
        {
            var attempts = logger.GetLogRecords(date);

            var totalLoginsCount = attempts.GroupBy(x => x.Login).Count();

            var invalidAttempts = attempts
                .Where(x => x.LoginStatus != AirBnbLoginStatus.Sucess)
                .GroupBy(x => x.Login)
                .ToList();

            var invalidProxies = attempts
                .Where(x => x.LoginStatus == AirBnbLoginStatus.ProxyError && !string.IsNullOrEmpty(x.Proxy))
                .Select(x => x.Proxy)
                .Distinct()
                .ToList();

            return new LoginStatistics()
            {
                ErrorsCount = invalidAttempts.Count(),
                ErrorLogins = invalidAttempts.Select(x => new
                {
                    x.Key,
                    Errors = string.Join(", ", x.GroupBy(l => l.LoginStatus)
                                                    .Select(s => string.Format("{0} ({1})", s.Key.ToDescription(), s.Count())).ToList())
                })
                .ToDictionary(x => x.Key, x => x.Errors),
                LoginCount = totalLoginsCount,
                InvalidProxies = invalidProxies
            };
        }
    }
}