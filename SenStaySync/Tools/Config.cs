namespace SenStaySync
{
    using Newtonsoft.Json;

    public class Config
    {
        public const string CONFIG_FILE_PATH = @"config.json";

        private static Config Instance;
        public string AirBnbAccountsFile = @"E:\Src\SenStay\Data\airbnb.json";
        public string AirBnbRoomsMapFile = @"E:\Src\SenStay\Data\airbnbrooms.json";
        public string AppDirictory = @"E:\Src\vs\SenStay\SS-AirBnb2Streamline\SS-AirBnb2Streamline";
        public string DailySeasons = @"E:\Src\SenStay\Data\seasons.json";
        //public string PricingPushFile = @"E:\Pricing Push\Streamline Data Test1.csv";
        public string PricingPushFile = @"E:\Pricing Push\Pricing Push Sample Data 1.csv";
        public string ErrorsEmail = @"error.notifications@senstay.com";
        public string NotificationsEmail = @"DL_Airbnb_Export_Report@senstay.com";
        public string PricingPushNotificationsEmail = "viswas.menon@nibodha.com";
        //@"sysops@senstay.com,revenue_team@senstay.com";
               public string ErrorsEmailDisplayName = @"SenStay Error Reporting";
        public string ErrorsEmailPassword = @"Xsf431X3i5KotLNT";
        public string ErrorsEmailSMTP = @"smtp.gmail.com";
        public string ExportCompletedTransactionsDirectory = @"E:\Src\SenStay\Data\Completed";
        public string ExportFutureTransactionsDirectory = @"E:\Src\SenStay\Data\Future";
        public string GoogleServiceAccountEmail = "id-85-372@senstay-1155.iam.gserviceaccount.com";
        public string ActualPasswordsSheetTitle = "Active Airbnb Accounts";
        public string ImagesFolder = @"E:\Src\SenStay\Data\Images\";
        public string LockDirectory = @"E:\Src\SenStay\Data\Lock";
        public string LoginAttemptsLogFile = @"E:\Src\SenStay\Data\Logs\loginAttempts.json";
        public string PricingPushAttemptsLogFile = @"E:\Src\SenStay\Data\Logs\PricingPush.json";

        public string NotiferClass = "SenStaySync.ConsoleNotifier";
        public string PasswordSyncLogFile = @"E:\Src\SenStay\Data\Logs\passSyncAttempts.json";
        public string PriceOldScriptsDirectory = @"E:\Src\SenStay\Data\Pricing\OldPriceScripts";
        public string PriceOldSourceDirectory = @"E:\Src\SenStay\Data\Pricing\OldSource";
        public string PriceScriptsDirectory = @"E:\Src\SenStay\Data\Pricing\PriceScripts";
        public string PriceSourceDirectory = @"E:\Src\SenStay\Data\Pricing\Source";

        public string PriceStatusDirectory = @"E:\Src\SenStay\Data\Pricing\StatusData";
        public string PropertyCollectionJsonPath = @"E:\Src\SenStay\Data\streamline.json";

        public string ProxyListFile = @"E:\Src\SenStay\Data\proxylist.json";
        public string ProxyMapFile = @"E:\Src\SenStay\Data\proxymap.json";
        public int RegularWaitSeconds = 10;
        public string TempDirectory = @"E:\Src\SenStay\Data\temp";

        public bool UseProxy = false;
        public string X509 = "Key.p12";
        public string FirefoxBinaryPath = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";

        public string StreamLineAccountLogin = "devtest1";
        public string StreamLineAccountPassword = "DevTest11";
        public bool PushPriceToStreamLine = true;
        public bool PushPriceToAirBnb = false;

        // Streamline URL's 
        public string StreamLineLoginPageURL = "https://admin.streamlinevrs.com/auth_login.html";
        public string StreamLineEditHomeURL = "https://admin.streamlinevrs.com/edit_home.html?home_id=";
        public string StreamLineCreateSeasonGroupURL = "https://admin.streamlinevrs.com/edit_company_season_group.html?id=0&lodging_type_id=3&copy_season_group=0";
        public string StreamLineCreateSeasonURL = "https://admin.streamlinevrs.com/edit_company_season.html?id=0&lodging_type_id=3";


        public static Config I
        {
            get
            {
                if (Instance == null)
                {
                    Instance = Load();
                }
                return Instance;
            }
        }

        public void Save()
        {
            this.SaveToFileAsJson(CONFIG_FILE_PATH, Formatting.Indented);
        }

        public static Config Load()
        {
            var ConfObj = FileUtils.LoadFromJsonFile<Config>(CONFIG_FILE_PATH);
            if (ConfObj == null)
            {
                if (FileUtils.FileExists(CONFIG_FILE_PATH))
                {
                    FileUtils.BackupFile(CONFIG_FILE_PATH);
                }

                ConfObj = new Config();
                ConfObj.Save();
            }

            Temp.TouchDirectory(ConfObj.TempDirectory);
            Temp.TouchDirectory(ConfObj.LockDirectory);
            Temp.TouchDirectory(ConfObj.ExportCompletedTransactionsDirectory);
            Temp.TouchDirectory(ConfObj.ExportFutureTransactionsDirectory);

            return ConfObj;
        }
    }
}