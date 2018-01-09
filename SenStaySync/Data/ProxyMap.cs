namespace SenStaySync.Data
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ProxyMap : ISavable
    {
        public readonly List<string> DefaultProxies = new List<string>
        {
            "173.234.226.10:8800"
        };

        private bool _changed;
        private DateTime _changedDate;

        public readonly Dictionary<string, List<string>> Map = new Dictionary<string, List<string>>();

        public void Save()
        {
            _changed = false;
            _changedDate = DateTime.UtcNow;
            this.SaveToFileAsJson(Config.I.ProxyMapFile, Formatting.Indented);
        }

        public static ProxyMap Load()
        {
            var map = LoadUtils.Load<ProxyMap>(Config.I.ProxyMapFile);
            map._changed = false;
            return map;
        }

        public void Commit()
        {
            if (_changed)
            {
                Save();
            }
        }

        public void Add(string email, List<string> proxies)
        {
            if (!Map.ContainsKey(email))
            {
                Map.Add(email, proxies);
            }
            else
            {
                Map[email] = proxies;
            }
        }

        public void Add(string email, string proxy)
        {
            Map.Add(email, new List<string> {proxy});
        }

        public List<string> GetProxy(string email, ProxyAssignStatistics proxyAssign)
        {
            if (Map.ContainsKey(email))
            {
                var proxyAddrList = Map[email];
                if (proxyAssign != null)
                {
                    proxyAssign.SetUsed(proxyAddrList);
                }
                return proxyAddrList;
            }

            if (proxyAssign == null)
                return DefaultProxies;

            var addr = proxyAssign.GetUnMapedAddress();

            if (string.IsNullOrWhiteSpace(addr))
                return DefaultProxies;

            _changed = true;
            Add(email, addr);
            proxyAssign.SetUsed(addr);
            return new List<string> {addr};
        }
    }

    public class ProxyAssignStatistics
    {
        public HashSet<string> MappedButUnused;
        public HashSet<string> MultipleUsed;
        public HashSet<string> NotMapped;

        public HashSet<string> Pool;
        public HashSet<string> Unused;
        public HashSet<string> Used;

        public void Setup(ProxyList ProxyList, ProxyMap ProxyMap)
        {
            Pool = new HashSet<string>();
            Used = new HashSet<string>();
            Unused = new HashSet<string>();
            MappedButUnused = new HashSet<string>();
            MultipleUsed = new HashSet<string>();
            NotMapped = new HashSet<string>();

            foreach (var item in ProxyList)
            {
                Pool.Add(item);
                Unused.Add(item);
                NotMapped.Add(item);
            }

            foreach (var List in ProxyMap.Map.Values)
            {
                foreach (var Address in List)
                {
                    MappedButUnused.Add(Address);
                    NotMapped.Remove(Address);
                }
            }
        }


        public void SetUsed(List<string> AddressList)
        {
            foreach (var Address in AddressList)
            {
                SetUsed(Address);
            }
        }

        public void SetUsed(string Address)
        {
            if (!Used.Add(Address))
            {
                MultipleUsed.Add(Address);
            }
            Unused.Remove(Address);
            MappedButUnused.Remove(Address);
            NotMapped.Remove(Address);
        }

        public string GetUnMapedAddress()
        {
            if (NotMapped.Count > 0)
            {
                foreach (var i in NotMapped)
                {
                    return i;
                }
            }
            return null;
        }
    }

    public static class ProxyExtensions
    {
        public static bool IsDefaultProxy(this List<string> proxyList, ProxyMap map)
        {
            foreach (var proxy in proxyList)
            {
                if (!map.DefaultProxies.Contains(proxy))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ContainsDefaultProxy(this List<string> proxyList, ProxyMap map)
        {
            foreach (var proxy in proxyList)
            {
                if (map.DefaultProxies.Contains(proxy))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class ProxyList : List<string>, ISavable
    {
        public void Save()
        {
            this.SaveToFileAsJson(Config.I.ProxyListFile, Formatting.Indented);
        }

        public static ProxyList Load()
        {
            return LoadUtils.Load<ProxyList>(Config.I.ProxyListFile);
        }
    }
}