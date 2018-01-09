using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SenStaySync.PageProcesser.Streamline;

namespace SenStaySync.Test.Scripts
{
    public class StreamlineLogin1
    {
        public static void Process()
        {
            var Dr = SeleniumFactory.GetFirefoxDriver();
            var Account = new Data.StreamlineAccount() { Login = "xxx", Password = "yyy" };
            StreamlineLogin.Process(Dr, Account, true);
        }


        public static void ProductionProcess()
        {
            var Dr = SeleniumFactory.GetFirefoxDriver();
            var Account = new Data.StreamlineAccount() { Login = "devtest1", Password = "DevTest11" };
            StreamlineLogin.Process(Dr, Account, false);
        }
    }

}

