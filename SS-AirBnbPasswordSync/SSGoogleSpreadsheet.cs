namespace SS_AirBnbPasswordSync
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using Google.Apis.Auth.OAuth2;
    using Google.GData.Client;
    using Google.GData.Spreadsheets;
    using SenStaySync;

    public static class SSGoogleSpreadsheet
    {
        public static List<AirBnbAccountCredentials> GetAirBnbPasswordsFromSpreadsheet()
        {
            var serviceAccountEmail = Config.I.GoogleServiceAccountEmail;
            var x509File = Config.I.X509;
            var certificate = new X509Certificate2(x509File, "notasecret", X509KeyStorageFlags.Exportable);

            var serviceAccountCredentialInitializer = new ServiceAccountCredential
                    .Initializer(serviceAccountEmail)
                        {
                            Scopes = new[] {
                                @"https://spreadsheets.google.com/feeds",
                                @"https://docs.google.com/feeds"
                            }
                        }
                    .FromCertificate(certificate);

            var credential = new ServiceAccountCredential(serviceAccountCredentialInitializer);

            if (!credential.RequestAccessTokenAsync(CancellationToken.None).Result)
            {
                throw new InvalidOperationException("Access token request failed.");
            }

            var requestFactory = new GDataRequestFactory(null);
            requestFactory.CustomHeaders.Add("Authorization: Bearer " + credential.Token.AccessToken);

            var service = new SpreadsheetsService("senstay") {RequestFactory = requestFactory};

            var query = new SpreadsheetQuery();
            var feed = service.Query(query);

            SpreadsheetEntry airPasswords = null;
            foreach (var atomEntry in feed.Entries)
            {
                var entry = (SpreadsheetEntry) atomEntry;
                var titleStripted = entry.Title.Text + "";

                if (!titleStripted.Contains("Airbnb Passwords for Nikita"))
                    continue;

                airPasswords = entry;
                break;
            }

            if (airPasswords == null)
            {
                return new List<AirBnbAccountCredentials>();
            }

            var link = airPasswords.Links.FindService(GDataSpreadsheetsNameTable.WorksheetRel, null);

            var queryW = new WorksheetQuery(link.HRef.ToString());
            var feedW = service.Query(queryW);

            var worksheet = feedW.Entries.Cast<WorksheetEntry>().FirstOrDefault(x => x.Title.Text == Config.I.ActualPasswordsSheetTitle);

            if (worksheet == null)
            {
                return new List<AirBnbAccountCredentials>();
            }

            var cellFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.CellRel, null);

            var queryC = new CellQuery(cellFeedLink.HRef.ToString());
            var feedC = service.Query(queryC);

            var passwords = new Dictionary<uint, AirBnbAccountCredentials>();

            foreach (var atomEntry in feedC.Entries)
            {
                var curCell = (CellEntry) atomEntry;

                var col = curCell.Cell.Column;
                var row = curCell.Cell.Row;
                var value = curCell.Cell.Value;

                var credentials = passwords.ContainsKey(row) ? passwords[row] : new AirBnbAccountCredentials();

                switch (col)
                {
                    case 1:
                        credentials.Email = value;
                        break;
                    case 2:
                        credentials.Password = value;
                        break;
                    case 4:
                    case 5:
                        if (!string.IsNullOrEmpty(value))
                            credentials.Proxies.Add(value);
                        break;
                }

                if (credentials.Email == null || !credentials.Email.Contains("@"))
                    continue;

                passwords[row] = credentials;
            }

            return GetList(passwords);
        }

        private static ListFeed GetListFeed()
        {
            var serviceAccountEmail = Config.I.GoogleServiceAccountEmail;
            var x509File = Config.I.X509;
            var certificate = new X509Certificate2(x509File, "notasecret", X509KeyStorageFlags.Exportable);

            var serviceAccountCredentialInitializer = new ServiceAccountCredential
                    .Initializer(serviceAccountEmail)
            {
                Scopes = new[] {
                                @"https://spreadsheets.google.com/feeds",
                                @"https://docs.google.com/feeds"
                            }
            }
                    .FromCertificate(certificate);

            var credential = new ServiceAccountCredential(serviceAccountCredentialInitializer);

            if (!credential.RequestAccessTokenAsync(CancellationToken.None).Result)
            {
                throw new InvalidOperationException("Access token request failed.");
            }

            var requestFactory = new GDataRequestFactory(null);
            requestFactory.CustomHeaders.Add("Authorization: Bearer " + credential.Token.AccessToken);

            var service = new SpreadsheetsService("senstay") { RequestFactory = requestFactory };

            var query = new SpreadsheetQuery();
            var feed = service.Query(query);

            SpreadsheetEntry airPasswords = null;
            foreach (var atomEntry in feed.Entries)
            {
                var entry = (SpreadsheetEntry)atomEntry;
                var titleStripted = entry.Title.Text + "";

                if (!titleStripted.Contains("Airbnb Passwords for Nikita"))
                    continue;

                airPasswords = entry;
                break;
            }

            var link = airPasswords.Links.FindService(GDataSpreadsheetsNameTable.WorksheetRel, null);

            var queryW = new WorksheetQuery(link.HRef.ToString());
            var feedW = service.Query(queryW);

            var worksheet = feedW.Entries.Cast<WorksheetEntry>().FirstOrDefault();

            var cellFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

            var queryC = new ListQuery(cellFeedLink.HRef.ToString());

            return service.Query(queryC);
        }

        private static List<AirBnbAccountCredentials> GetList(Dictionary<uint, AirBnbAccountCredentials> credentials)
        {
            return credentials.Values.ToList();
        }
    }
}