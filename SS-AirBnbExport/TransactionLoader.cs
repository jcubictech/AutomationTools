    namespace SS_AirBnbExport
{
    using System;
    using log4net;
    using OpenQA.Selenium;
    using SenStaySync;
    using SenStaySync.Data;
    using SenStaySync.PageProcesser.AirBnb;
    using SenStaySync.Tools;

    internal class TransactionLoader
    {
        private readonly AirBnbAccount _airAccount;
        private readonly OperationsJsonLogger<LoginAttempt> _loginAttemptsProcessor;
        private readonly ILog _logger;

        internal TransactionLoader(AirBnbAccount airAccount, OperationsJsonLogger<LoginAttempt> loginAttemptsProcessor, ILog logger)
        {
            _airAccount = airAccount;
            _loginAttemptsProcessor = loginAttemptsProcessor;
            _logger = logger;
        }

        internal void DownloadTransactions()
        {
            _logger.Info("preparing locks");
            var account = _airAccount;
            var email = account.Email;
            var date = DateTime.Now;

            var lockHash = date.ToString("yyyy\\MMMM-d") + @"\" + email;
            var lockHashSuccess = lockHash + "-sucess";

            if (Temp.IsLocked(lockHashSuccess))
            {
                _logger.Info(string.Format("{0} is processed already", email));
                N.Note(email + " skiped");
                return;
            }

            foreach (var proxy in account.ProxyAddress)
            {
                var lockHashLoginError = string.Format("{0}-{1}-loginerror",lockHash, proxy.Replace(":","-"));

                _logger.Info(string.Format("Processing account {0} with proxy {1}", email, proxy));
                N.Note("Processing account " + email + " with proxy " + proxy);

                if (Temp.IsLocked(lockHashLoginError))
                {
                    _logger.Info(string.Format("{0} with proxy {1} is locked because of incorrect password", email, proxy));
                    N.Note(email + " (wrong password record)");
                    continue;
                }

                var profile = SeleniumProxy.GetFirefoxProfileWithProxy(proxy);
                _logger.Info("Prepare profile for downloading");
                AirBnbTransactionHistory.PrepareProfileForDownloading(profile);

                try
                {
                    _logger.Info("Creating driver");
                    using (var driver = SeleniumFactory.GetFirefoxDriver(profile))
                    {
                        _logger.Info("Signing in");
                        var signInSucceed = AirBnbLogin.Process(driver, account);

                        _logger.Info(string.Format("Sign in result: {0}", signInSucceed));
                        _loginAttemptsProcessor.Log(new LoginAttempt(email, signInSucceed.Status, signInSucceed.Message, proxy));

                        switch (signInSucceed.Status)
                        {
                            case AirBnbLoginStatus.Failed:
                            case AirBnbLoginStatus.Unable:
                            case AirBnbLoginStatus.VerifyUser:
                            case AirBnbLoginStatus.VerifyBrowser:
                                N.Note("Password for " + email + " is wrong or account unreachable");
                                Temp.Lock(lockHashLoginError);
                                continue;
                            case AirBnbLoginStatus.Sucess:
                                break;
                            case AirBnbLoginStatus.ProxyError:
                                break;
                            case AirBnbLoginStatus.PageError:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        _logger.Info("Prepare profile for downloading");
                        var completedLoadResult = LoadCompletedCsv(driver, date, email);

                        _logger.Info("Prepare profile for downloading");
                        var pendingLoadResult = LoadPendingCsv(driver, date, email);

                        if (!completedLoadResult || !pendingLoadResult)
                            continue;

                        Temp.Lock(lockHashSuccess);
                        _logger.InfoFormat("Account {0} is locked", account.Email);
                        break;
                    }

                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("Error while processing {0} with proxy {1}", account.Email, proxy), ex);
                    N.Note(email + " exception " + (ex + "").Substring(0, 70) + "...");
                }
            }
        }

        private bool LoadPendingCsv(IWebDriver driver, DateTime date, string email)
        {
            _logger.Info("Looking for completed transactions");
            Temp.EmptyFolder(Config.I.TempDirectory);
            var futureResult = AirBnbTransactionHistory.DownloadFutureTransaction(driver, date.Year);

            if (!futureResult)
            {
                _logger.Warn(string.Format("{0} doesn't have pending transactions", email));
                return true;
            }

            _logger.Info("downloading csv file");
            driver.JustWait(5);
            var pendingTransactionFileName = email + "-airbnb_pending.csv";
            var pendingCsvFile = Temp.WaitFotTheFirstFile("*.csv", Config.I.TempDirectory, 1000*60*5);

            //TODO: check is file have size 0kb then redo downloading
            if (pendingCsvFile == null)
            {
                _logger.Error("Error during file download");
                N.Note("error downloading file");
                return false;
            }

            var pendingTransactionsDirPath = Config.I.ExportFutureTransactionsDirectory + @"\Future Transactions - " + date.ToString("MMMM d yyyy") + @"\";
            Temp.TouchDirectory(pendingTransactionsDirPath);
            var pendingCsvFullPath = pendingTransactionsDirPath + @"\" + pendingTransactionFileName;
            Temp.Move(pendingCsvFile, pendingCsvFullPath);
            N.Note(pendingTransactionFileName + " downloaded");
            _logger.Info(pendingTransactionFileName + " downloaded");
            return true;
        }

        private bool LoadCompletedCsv(IWebDriver driver, DateTime date, string email)
        {
            Temp.EmptyFolder(Config.I.TempDirectory);

            _logger.Info("Looking for completed transactions");
            var completedResult = AirBnbTransactionHistory.DownloadCompletedTransaction(driver, date.Year);
            if (!completedResult)
            {
                _logger.Warn(string.Format("{0} doesn't have completed transactions", email));
                return true;
            }

            _logger.Info("downloading csv file");
            driver.JustWait(5);
            var completedTransactionFileName = email + "-airbnb.csv";
            var completedCsvFile = Temp.WaitFotTheFirstFile("*.csv", Config.I.TempDirectory, 1000*60*5);

            //TODO: check is file have size 0kb then redo downloading

            if (completedCsvFile == null)
            {
                _logger.Error("Error during file download");
                N.Note("error downloading file");
                return false;
            }

            var completedTransactionsDirPath = Config.I.ExportCompletedTransactionsDirectory + @"\" + date.ToString("MMMM d yyyy") + @"\";
            Temp.TouchDirectory(completedTransactionsDirPath);
            var completedCsvFullPath = completedTransactionsDirPath + @"\" + completedTransactionFileName;
            Temp.Move(completedCsvFile, completedCsvFullPath);
            N.Note(completedTransactionFileName + " downloaded");
            _logger.Info(completedTransactionFileName + " downloaded");
            return true;
        }
    }
}