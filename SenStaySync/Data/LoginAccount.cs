using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenStaySync.Data
{
    public class LoginAccount
    {
        public bool Active = true;
        public string Email;
        public string Password;
        public List<string> ProxyAddress;
        public bool Test = false;
    }
}
