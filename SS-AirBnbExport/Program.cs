namespace SS_AirBnbExport
{
    using System;
    using System.Linq;
    using log4net;
    using SenStaySync;
    using SenStaySync.Data;
    using SenStaySync.Tools;

    class Program
    {
        static void Main(string[] args)
        {
            Logger.Setup();
            Config.I.UseProxy = true;

            AirBnbAccounts.Load();

            var yesterday = DateTime.Now.AddDays(-1);
            var yesterdayLockHash = yesterday.ToString("yyyy\\MMMM-d") + @"\lock";

            var jsonLogger = new OperationsJsonLogger<LoginAttempt>(Config.I.LoginAttemptsLogFile);

            Temp.UnlockFolder(yesterdayLockHash);

            var logger = LogManager.GetLogger(typeof (Program));
            var accounts = AirBnbAccounts.Accounts.Where(x => !string.IsNullOrEmpty(x.Password)).ToList();

            foreach (var airAccount in accounts)
            {
                logger.Info(string.Format("processing {0} is started", airAccount.Email));
                var transactionLoader = new TransactionLoader(airAccount, jsonLogger, logger);
                transactionLoader.DownloadTransactions();
            }
        }
    }
}