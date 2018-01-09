namespace SenStaySync.Data
{
    using System;
    using System.ComponentModel;

    public class PasswordSyncError
    {
        public PasswordSyncError(PasswordErrorType errorType)
        {
            ErrorType = errorType;
        }

        public DateTime Date { get; set; }
        public PasswordErrorType ErrorType { get; set; }
    }

    public enum PasswordErrorType
    {
        [Description("Google spreadsheet connection error")]
        SpreadsheetConnectionError,
        [Description("Error while saving password to file")]
        SaveDataError
    }
}