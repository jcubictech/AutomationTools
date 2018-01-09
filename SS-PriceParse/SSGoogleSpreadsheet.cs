using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

using Google.Apis.Auth.OAuth2;
using Google.GData.Spreadsheets;
using Google.GData.Client;


using System.Collections.Generic;
using SenStaySync;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS_AirBnbPasswordSync
{
    public static class SSGoogleSpreadsheet
    {
        public static List<Tuple<string, string>> GetAirBnbPasswordsFromSpreadsheet()
        {
            string ServiceAccountEmail = Config.I.GoogleServiceAccountEmail;
            string x509File = Config.I.X509;
            var certificate = new X509Certificate2(x509File, "notasecret", X509KeyStorageFlags.Exportable);

            var serviceAccountCredentialInitializer =
                new ServiceAccountCredential.Initializer(ServiceAccountEmail)
                {
                    Scopes = new[] { @"https://spreadsheets.google.com/feeds", @"https://docs.google.com/feeds" }
                }.FromCertificate(certificate);

            var credential = new ServiceAccountCredential(serviceAccountCredentialInitializer);

            if (!credential.RequestAccessTokenAsync(System.Threading.CancellationToken.None).Result)
                throw new InvalidOperationException("Access token request failed.");

            var requestFactory = new GDataRequestFactory(null);
            requestFactory.CustomHeaders.Add("Authorization: Bearer " + credential.Token.AccessToken);

            //var service = new SpreadsheetsService(null) { RequestFactory = requestFactory };
            SpreadsheetsService service = new SpreadsheetsService("senstay");
            service.RequestFactory = requestFactory;

            SpreadsheetQuery query = new SpreadsheetQuery();
            SpreadsheetFeed feed = service.Query(query);

            //Console.WriteLine("Your spreadsheets:");
            SpreadsheetEntry AirPasswords = null;
            foreach (SpreadsheetEntry entry in feed.Entries)
            {
                var TitleStripted = entry.Title.Text + "";
                if (TitleStripted.Contains("Airbnb Passwords for Nikita"))
                {
                    AirPasswords = entry;
                    break;
                }
                //Console.WriteLine(entry.Title.Text);
            }

            AtomLink link = AirPasswords.Links.FindService(GDataSpreadsheetsNameTable.WorksheetRel, null);

            WorksheetQuery queryW = new WorksheetQuery(link.HRef.ToString());
            WorksheetFeed feedW = service.Query(queryW);

            WorksheetEntry Worksheet = null;

            foreach (WorksheetEntry worksheet in feedW.Entries)
            {
                Worksheet = worksheet;
                break;
                //Console.WriteLine(worksheet.Title.Text);
            }

            AtomLink cellFeedLink = Worksheet.Links.FindService(GDataSpreadsheetsNameTable.CellRel, null);

            CellQuery queryC = new CellQuery(cellFeedLink.HRef.ToString());
            CellFeed feedC = service.Query(queryC);

            //Console.WriteLine("Cells in this worksheet:");

            var Passwords = new Dictionary<uint, Tuple<string, string>>();

            foreach (CellEntry curCell in feedC.Entries)
            {
                //Console.WriteLine("Row {0}, column {1}: {2}", curCell.Cell.Row, curCell.Cell.Column, curCell.Cell.Value);

                var col = curCell.Cell.Column;
                var row = curCell.Cell.Row;
                var value = curCell.Cell.Value;
                Tuple<string, string> EmailAndPassword = null;
                if (Passwords.ContainsKey(row))
                {
                    EmailAndPassword = Passwords[row];
                }
                else
                {
                    EmailAndPassword = new Tuple<string, string>("", "");
                }

                if (col == 1)
                {
                    EmailAndPassword = new Tuple<string, string>(value, EmailAndPassword.Item2);
                }
                else if (col == 2)
                {
                    EmailAndPassword = new Tuple<string, string>(EmailAndPassword.Item1, value);
                }

                if (!EmailAndPassword.Item1.Contains("@")) continue;

                Passwords[row] = EmailAndPassword;
            }

            /*
            foreach (var i in Passwords)
            {
                Console.WriteLine(i.Key + ") " + i.Value.Item1 + " = " + i.Value.Item2);
            }
            //*/
            //Console.WriteLine(Passwords.Values);
            //Console.ReadLine();

            return GetList(Passwords);
        }

        private static List<Tuple<string, string>> GetList(Dictionary<uint, Tuple<string, string>> Passwords)
        {
            var List = new List<Tuple<string, string>>();
            foreach (var i in Passwords.Values)
            {
                List.Add(new Tuple<string, string>(i.Item1, i.Item2));
            }
            return List;
        }
    }
}
