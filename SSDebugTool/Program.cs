namespace SSDebugTool
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using SenStaySync;
    using SenStaySync.Data;

    class Program
    {
        static void Main(string[] args)
        {
            //N.Note("LA099".ExtractSenStayID());
            //N.Note("D011".ExtractSenStayID());
            //N.Note("D11".ExtractSenStayID());
            //N.Note("LA100".ExtractSenStayID());
            //N.Note("LA101".ExtractSenStayID());
            //N.Note("LA01".ExtractSenStayID());
            //Console.ReadLine();

            //EmailNotification.Send("berd.na@gmail.com", "Test", "The test body");

            Config.I.UseProxy = true;

            DataInit.Init();
            AirBnbAccounts.Load();
            AirBnbAccounts.Save();
            //return;

            var accounts = AirBnbAccounts.Accounts;
            var proxyMap = DataInit.proxyMap;

            var containsDefaulProxyList = new List<AirBnbAccount>();
            var defaultProxylList = new List<AirBnbAccount>();
            var mappedAccountsList = new List<AirBnbAccount>();

            var unusedAddreses = new List<List<string>>();
            var usedAddreses = new List<List<string>>();


            //
            foreach (var account in accounts)
            {
                if (account.ProxyAddress.IsDefaultProxy(proxyMap))
                {
                    defaultProxylList.Add(account);
                }
                else if (account.ProxyAddress.ContainsDefaultProxy(proxyMap))
                {
                    containsDefaulProxyList.Add(account);
                }
                else
                {
                    mappedAccountsList.Add(account);
                }
            }

            var accountsMap = accounts.GetMap();

            foreach (var mapped in proxyMap.Map)
            {
                if (accountsMap.ContainsKey(mapped.Key))
                {
                    usedAddreses.Add(mapped.Value);
                }
                else
                {
                    unusedAddreses.Add(mapped.Value);
                }
            }

            var proxyPool = new Stack<string>();
            var proxyHashSet = new HashSet<string>();

            foreach (var addresList in unusedAddreses)
            {
                foreach (var address in addresList)
                {
                    proxyHashSet.Add(address);
                }
            }

            foreach (var account in containsDefaulProxyList)
            {
                foreach (var proxy in account.ProxyAddress)
                {
                    if (!proxyMap.DefaultProxies.Contains(proxy))
                    {
                        proxyHashSet.Add(proxy);
                    }
                }
            }

            var newMap = new Dictionary<string, List<string>>();

            foreach (var account in mappedAccountsList)
            {
                newMap.Add(account.Email, new List<string> {account.ProxyAddress[0]});
                for (var i = 1; i < account.ProxyAddress.Count; i++)
                {
                    proxyPool.Push(account.ProxyAddress[i]);
                }
            }

            foreach (var proxy in proxyHashSet)
            {
                proxyPool.Push(proxy);
            }

            foreach (var account in containsDefaulProxyList)
            {
                var proxy = "default"; //ProxyMap.DefaultProxies[0];

                if (proxyPool.Count > 0)
                {
                    proxy = proxyPool.Pop();
                }

                newMap.Add(account.Email, new List<string> {proxy});
            }

            foreach (var account in defaultProxylList)
            {
                var proxy = "default"; //ProxyMap.DefaultProxies[0];

                if (proxyPool.Count > 0)
                {
                    proxy = proxyPool.Pop();
                }

                newMap.Add(account.Email, new List<string> {proxy});
            }

            var code = new List<string>();
            foreach (var assign in newMap)
            {
                var email = assign.Key;
                var proxyList = assign.Value;
                var pList = proxyList.Select(proxy => '"' + proxy + '"').ToList();

                string.Join(", ", pList);

                var assignCodeLine = "p.Add(\"" + email + "\", new List<string>() { " + string.Join(", ", pList) + " });";

                code.Add(assignCodeLine);
            }

            FileUtils.SaveTextToFile("debug.newMapCode.txt", string.Join("\n", code));

            containsDefaulProxyList.SaveToFileAsJson("debug.ContainsDefaulProxyList.json", Formatting.Indented);
            defaultProxylList.SaveToFileAsJson("debug.DefaultProxylList.json", Formatting.Indented);
            mappedAccountsList.SaveToFileAsJson("debug.MappedAccountsList.json", Formatting.Indented);
            proxyPool.SaveToFileAsJson("debug.ProxyPool.json", Formatting.Indented);
            newMap.SaveToFileAsJson("debug.NewMap.json", Formatting.Indented);

            unusedAddreses.SaveToFileAsJson("debug.UnusedAddreses.json", Formatting.Indented);
            usedAddreses.SaveToFileAsJson("debug.UsedAddreses.json", Formatting.Indented);

            //Formatting.Indented

            AirBnbAccounts.Save();
        }
    }
}