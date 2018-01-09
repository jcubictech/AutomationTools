namespace SS_AirBnbPasswordSync
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SenStaySync.Data;

    public static class AirBnbAccountsConfigBuilder
    {
        public static ProxyMap ProxyMap = new ProxyMap();
        public static ProxyList ProxyList = new ProxyList();
        public static ProxyAssignStatistics ProxyAssignStat = new ProxyAssignStatistics();

        public static void Init()
        {
            ProxyMap = ProxyMap.Load();
            ProxyList = ProxyList.Load();
            ProxyAssignStat = new ProxyAssignStatistics();
            ProxyAssignStat.Setup(ProxyList, ProxyMap);
        }

        public static void Save()
        {
            ProxyMap.Commit();
            AirBnbAccounts.Save();
        }

        public static void SetAccountCredentials(List<AirBnbAccountCredentials> credentials)
        {
            foreach (var credential in credentials)
            {
                if (credential.Proxies.Any())
                {
                    ProxyMap.Add(credential.Email, credential.Proxies);
                }

                try
                {
                    AirBnbAccounts.AddAccount(credential.Email, credential.Password, ProxyMap, ProxyAssignStat);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public static void SetProxyList()
        {
        }
    }
}