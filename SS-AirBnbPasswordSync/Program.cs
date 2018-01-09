namespace SS_AirBnbPasswordSync
{
    using System;
    using System.Collections.Generic;
    using SenStaySync;
    using SenStaySync.Data;
    using SenStaySync.Tools;

    class Program
    {
        static void Main(string[] args)
        {
            List<AirBnbAccountCredentials> passwords = null;
            var logger = new OperationsJsonLogger<PasswordSyncError>(Config.I.PasswordSyncLogFile);

            try
            {
                Config.I.UseProxy = true;
                passwords = SSGoogleSpreadsheet.GetAirBnbPasswordsFromSpreadsheet();
            }
            catch (Exception e)
            {
                logger.Log(new PasswordSyncError(PasswordErrorType.SpreadsheetConnectionError));
            }

            if (passwords == null)
                return;

            try
            {
                AirBnbAccountsConfigBuilder.Init();
                AirBnbAccountsConfigBuilder.SetAccountCredentials(passwords);
                AirBnbAccountsConfigBuilder.Save();
            }
            catch (Exception e)
            {
                logger.Log(new PasswordSyncError(PasswordErrorType.SaveDataError));
            }
        }
    }
}