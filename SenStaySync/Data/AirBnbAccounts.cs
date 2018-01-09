namespace SenStaySync.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class AirBnbAccounts
    {
        internal static AirBnbAccountMap AccountMap = new AirBnbAccountMap();
        public static AirBnbAccountsList Accounts = new AirBnbAccountsList();

        public static AirBnbAccount GetAccountByEmail(string Email)
        {
            //TODO: Database
            return AccountMap.ContainsKey(Email) ? AccountMap[Email] : null;
        }

        public static void Save()
        {
            AccountMap.GetList().Save();
        }

        public static void Load()
        {
            Accounts = AirBnbAccountsList.Load();
            AccountMap = Accounts.GetMap();
        }

        public static void AddAccount(AirBnbAccount item)
        {
            AccountMap.Add(item.Email, item);
            Accounts.Add(item);
        }

        public static void AddAccount(string email, string password, ProxyMap proxyMap, ProxyAssignStatistics proxyAssignStat = null)
        {
            AddAccount(new AirBnbAccount
            {
                Email = email,
                Password = password,
                ProxyAddress = proxyMap.GetProxy(email, proxyAssignStat)
            });
        }

        public static void AddAccount(string email, string password, string proxyAddress)
        {
            AddAccount(new AirBnbAccount
            {
                Email = email,
                Password = password,
                ProxyAddress = new List<string> {proxyAddress}
            });
        }

        public static void AddAccount(string email, string password, List<string> proxyAddress)
        {
            AddAccount(new AirBnbAccount {Email = email, Password = password, ProxyAddress = proxyAddress});
        }
    }

    public class AirBnbAccountsList : List<AirBnbAccount>, ISavable
    {
        public void Save()
        {
            this.SaveToFileAsJson(Config.I.AirBnbAccountsFile, Formatting.Indented);
        }

        public static AirBnbAccountsList Load()
        {
            return LoadUtils.Load<AirBnbAccountsList>(Config.I.AirBnbAccountsFile);
        }
    }

    public class AirBnbAccountMap : Dictionary<string, AirBnbAccount>
    {
    }

    public static class AirBnbAccountExtensions
    {
        public static AirBnbAccountsList GetList(this AirBnbAccountMap accounts)
        {
            var list = new AirBnbAccountsList();
            list.AddRange(accounts.Select(item => item.Value));
            return list;
        }

        public static AirBnbAccountMap GetMap(this AirBnbAccountsList accounts)
        {
            var map = new AirBnbAccountMap();
            foreach (var item in accounts)
            {
                map.Add(item.Email, item);
            }
            return map;
        }
    }

    public class AirBnbAccount
    {
        public bool Active = true;
        public string Email;
        public string Password;
        public List<string> ProxyAddress;
        public bool Test = false;
    }
}