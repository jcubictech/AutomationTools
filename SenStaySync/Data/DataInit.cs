namespace SenStaySync.Data
{
    using System.Collections.Generic;

    public class DataInit
    {
        public static ProxyMap proxyMap = new ProxyMap();

        public static void Init()
        {
            var p = new ProxyMap();
            p.Add("adamrentals1@gmail.com", new List<string> {"206.214.93.136:8800"});
            p.Add("benrentals1@gmail.com", new List<string> {"173.234.59.205:8800"});
            p.Add("brandy.b.bryant@gmail.com", new List<string> {"69.147.248.248:8800"});
            p.Add("brianairbnb1@gmail.com", new List<string> {"69.147.248.202:8800"});
            p.Add("bryansrentals1@gmail.com", new List<string> {"173.234.226.64:8800"});
            p.Add("carlorentals1@gmail.com", new List<string> {"69.147.248.5:8800"});
            p.Add("carlyrentals1@gmail.com", new List<string> {"170.130.63.26:8800"});
            p.Add("cheyennerentals1@gmail.com", new List<string> {"173.234.165.54:8800"});
            p.Add("jesseairbnb2@gmail.com", new List<string> {"206.214.93.110:8800"});
            p.Add("jesserentals1@gmail.com", new List<string> {"173.234.226.51:8800"});
            p.Add("joncairbnb1@gmail.com", new List<string> {"173.232.7.240:8800"});
            p.Add("jonrentals1@gmail.com", new List<string> {"206.214.93.106:8800"});
            p.Add("joshrentals1@gmail.com", new List<string> {"173.234.165.64:8800"});
            p.Add("katierentals1@gmail.com", new List<string> {"50.31.9.124:8800"});
            p.Add("mattlucidorentals1@gmail.com", new List<string> {"170.130.63.128:8800"});
            p.Add("mojdehrentals1@gmail.com", new List<string> {"50.31.9.228:8800"});
            p.Add("rickrentalninjas@gmail.com", new List<string> {"50.31.9.67:8800"});
            p.Add("robbieairbnb1@gmail.com", new List<string> {"173.234.165.233:8800"});
            p.Add("roccorentals1@gmail.com", new List<string> {"50.31.9.245:8800"});
            p.Add("sashaairbnb1@gmail.com", new List<string> {"170.130.63.162:8800"});
            p.Add("vieverentals1@gmail.com", new List<string> {"50.31.9.36:8800"});
            p.Add("donaldairbnb1@gmail.com", new List<string> {"173.232.7.18:8800"});
            p.Add("tylerrentals1@gmail.com", new List<string> {"206.214.93.126:8800"});
            p.Add("amishrentals1@gmail.com", new List<string> {"173.232.7.235:8800"});
            p.Add("anhrentals1@gmail.com", new List<string> {"206.214.93.225:8800"});
            p.Add("dmurphyrentals1@gmail.com", new List<string> {"170.130.63.73:8800"});
            p.Add("donnarentals2@gmail.com", new List<string> {"173.234.59.83:8800"});
            p.Add("jasonrentals1@gmail.com", new List<string> {"69.147.248.46:8800"});
            p.Add("nickrentals11@gmail.com", new List<string> {"170.130.63.217:8800"});
            p.Add("oliverrentals1@gmail.com", new List<string> {"206.214.93.83:8800"});
            p.Add("robertrentals4@gmail.com", new List<string> {"170.130.63.13:8800"});
            p.Add("vitalyrentals1@gmail.com", new List<string> {"173.234.226.192:8800"});


            // Old mapping
            /*
            p.Add("sashaairbnb1@gmail.com", new List<string>() { "170.130.63.162:8800", "173.234.59.83:8800" });
            p.Add("mattlucidorentals1@gmail.com", new List<string>() { "170.130.63.128:8800", "173.234.226.192:8800" });
            p.Add("benrentals1@gmail.com", new List<string>() { "173.234.59.205:8800", "173.234.226.5:8800" });
            p.Add("adamrentals1@gmail.com", new List<string>() { "206.214.93.136:8800", "173.232.7.98:8800" });
            p.Add("robbieairbnb1@gmail.com", new List<string>() { "173.234.165.233:8800", "170.130.63.217:8800" });
            p.Add("brianairbnb1@gmail.com", new List<string>() { "69.147.248.202:8800", "173.234.59.53:8800" });
            p.Add("cheyennerentals1@gmail.com", new List<string>() { "173.234.165.54:8800", "50.31.9.142:8800" });
            p.Add("jesserentals1@gmail.com", new List<string>() { "173.234.226.51:8800", "206.214.93.250:8800" });
            p.Add("joshrentals1@gmail.com", new List<string>() { "173.234.165.64:8800", "173.234.59.186:8800" });

            p.Add("patrickrentalninjas@gmail.com", new List<string>() { "206.214.93.225:8800", "173.232.7.235:8800" });

            p.Add("vieverentals1@gmail.com", new List<string>() { "50.31.9.36:8800", "170.130.63.73:8800" });
            p.Add("bryansrentals1@gmail.com", new List<string>() { "173.234.226.64:8800", "173.234.165.53:8800" });
            p.Add("jesseairbnb2@gmail.com", new List<string>() { "206.214.93.110:8800", "173.234.165.52:8800" });
            p.Add("jonrentals1@gmail.com", new List<string>() { "206.214.93.106:8800", "173.232.7.154:8800" });
            p.Add("mojdehrentals1@gmail.com", new List<string>() { "50.31.9.228:8800", "170.130.63.13:8800" });
            p.Add("rickrentalninjas@gmail.com", new List<string>() { "50.31.9.67:8800", "206.214.93.83:8800" });
            p.Add("brandy.b.bryant@gmail.com", new List<string>() { "69.147.248.248:8800", "69.147.248.112:8800" });
            p.Add("carlorentals1@gmail.com", new List<string>() { "69.147.248.5:8800", "206.214.93.183:8800" });
            p.Add("carlyrentals1@gmail.com", new List<string>() { "170.130.63.26:8800", "173.234.226.48:8800" });
            p.Add("joncairbnb1@gmail.com", new List<string>() { "173.232.7.240:8800", "173.234.59.55:8800" });
            p.Add("katierentals1@gmail.com", new List<string>() { "50.31.9.124:8800", "69.147.248.209:8800" });
            p.Add("roccorentals1@gmail.com", new List<string>() { "50.31.9.245:8800", "69.147.248.46:8800" });
            p.Add("tylerrentals1@gmail.com", new List<string>() { "173.232.7.18:8800", "173.234.226.10:8800" });
            p.Add("donaldairbnb1@gmail.com", new List<string>() { "206.214.93.126:8800", "173.234.165.33:8800" });
            //*/

            proxyMap = p;

            //Data.AirBnbAccounts.AddAccount("", "", p);

            AirBnbAccounts.AddAccount("adamrentals1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("amishrentals1@gmail.com", "sasha123", p);
            AirBnbAccounts.AddAccount("anhrentals1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("benrentals1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("brandy.b.bryant@gmail.com", "blabla123", p);
            AirBnbAccounts.AddAccount("brianairbnb1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("bryansrentals1@gmail.com", "lolpol88", p);
            AirBnbAccounts.AddAccount("carlorentals1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("carlyrentals1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("cheyennerentals1@gmail.com", "sasha123", p);
            AirBnbAccounts.AddAccount("dmurphyrentals1@gmail.com", "sasha123", p);
            AirBnbAccounts.AddAccount("donaldairbnb1@gmail.com", "westenble5a", p);
            AirBnbAccounts.AddAccount("donnarentals2@gmail.com", "Mother1@", p);
            AirBnbAccounts.AddAccount("jasonrentals1@gmail.com", "lolpol98!", p);
            AirBnbAccounts.AddAccount("jesseairbnb2@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("jesserentals1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("joncairbnb1@gmail.com", "lolpol99", p);
            AirBnbAccounts.AddAccount("jonrentals1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("joshrentals1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("katierentals1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("mattlucidorentals1@gmail.com", "AJ53Q~Nu%QBe2%\"j", p);
            AirBnbAccounts.AddAccount("mojdehrentals1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("nickrentals11@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("oliverrentals1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("rickrentalninjas@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("robbieairbnb1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("robertrentals4@gmail.com", "jr45208542", p);
            AirBnbAccounts.AddAccount("roccorentals1@gmail.com", "Sasha123", p);
            AirBnbAccounts.AddAccount("sashaairbnb1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("tylerrentals1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("vieverentals1@gmail.com", "lolpol98", p);
            AirBnbAccounts.AddAccount("vitalyrentals1@gmail.com", "f6&bFQ@L", p);


            /*
            Data.AirBnbAccounts.AddAccount("sashaairbnb1@gmail.com", "lolpol98", new List<string>() { "170.130.63.162:8800", "173.234.59.83:8800" });

            Data.AirBnbAccounts.AddAccount("mattlucidorentals1@gmail.com", "uJ90xePgCJR0sPxZKwrj", new List<string>() { "170.130.63.128:8800", "173.234.226.192:8800" });
            Data.AirBnbAccounts.AddAccount("benrentals1@gmail.com", "lolpol98", new List<string>() { "173.234.59.205:8800", "173.234.226.5:8800" });
            Data.AirBnbAccounts.AddAccount("adamrentals1@gmail.com", "lolpol98", new List<string>() { "206.214.93.136:8800", "173.232.7.98:8800" });
            Data.AirBnbAccounts.AddAccount("robbieairbnb1@gmail.com", "lolpol98", new List<string>() { "173.234.165.233:8800", "170.130.63.217:8800" });
            Data.AirBnbAccounts.AddAccount("brianairbnb1@gmail.com", "lolpol98", new List<string>() { "69.147.248.202:8800", "173.234.59.53:8800" });
            Data.AirBnbAccounts.AddAccount("cheyennerentals1@gmail.com", "sasha123", new List<string>() { "173.234.165.54:8800", "50.31.9.142:8800" });
            Data.AirBnbAccounts.AddAccount("jesserentals1@gmail.com", "lolpol98", new List<string>() { "173.234.226.51:8800", "206.214.93.250:8800" });
            Data.AirBnbAccounts.AddAccount("joshrentals1@gmail.com", "lolpol98", new List<string>() { "173.234.165.64:8800", "173.234.59.186:8800" });
            Data.AirBnbAccounts.AddAccount("patrickrentalninjas@gmail.com", "3B1Uxg80ISeK3CrxQI3G", new List<string>() { "206.214.93.225:8800", "173.232.7.235:8800" });
            Data.AirBnbAccounts.AddAccount("vieverentals1@gmail.com", "lolpol98", new List<string>() { "50.31.9.36:8800", "170.130.63.73:8800" });
            Data.AirBnbAccounts.AddAccount("bryansrentals1@gmail.com", "lolpol98", new List<string>() { "173.234.226.64:8800", "173.234.165.53:8800" });
            Data.AirBnbAccounts.AddAccount("jesseairbnb2@gmail.com", "lolpol98", new List<string>() { "206.214.93.110:8800", "173.234.165.52:8800" });
            Data.AirBnbAccounts.AddAccount("jonrentals1@gmail.com", "lolpol98", new List<string>() { "206.214.93.106:8800", "173.232.7.154:8800" });
            Data.AirBnbAccounts.AddAccount("mojdehrentals1@gmail.com", "lolpol98", new List<string>() { "50.31.9.228:8800", "170.130.63.13:8800" });
            Data.AirBnbAccounts.AddAccount("rickrentalninjas@gmail.com", "b@eB6&Ob5buimNmicCDA", new List<string>() { "50.31.9.67:8800", "206.214.93.83:8800" });
            Data.AirBnbAccounts.AddAccount("brandy.b.bryant@gmail.com", "blabla123", new List<string>() { "69.147.248.248:8800", "69.147.248.112:8800" });
            Data.AirBnbAccounts.AddAccount("carlorentals1@gmail.com", "lolpol98", new List<string>() { "69.147.248.5:8800", "206.214.93.183:8800" });
            Data.AirBnbAccounts.AddAccount("carlyrentals1@gmail.com", "lolpol98", new List<string>() { "170.130.63.26:8800", "173.234.226.48:8800" });
            Data.AirBnbAccounts.AddAccount("joncairbnb1@gmail.com", "lolpol98", new List<string>() { "173.232.7.240:8800", "173.234.59.55:8800" });
            Data.AirBnbAccounts.AddAccount("katierentals1@gmail.com", "lolpol98", new List<string>() { "50.31.9.124:8800", "69.147.248.209:8800" });
            Data.AirBnbAccounts.AddAccount("roccorentals1@gmail.com", "Sasha123", new List<string>() { "50.31.9.245:8800", "69.147.248.46:8800" });
            Data.AirBnbAccounts.AddAccount("tylerrentals1@gmail.com", "lolpol98", new List<string>() { "173.232.7.18:8800", "173.234.226.10:8800" });
            Data.AirBnbAccounts.AddAccount("donaldairbnb1@gmail.com", "westenble5a", new List<string>() { "206.214.93.126:8800", "173.234.165.33:8800" });

            

            ///
            /// This two proxy addresses used more than for 1 account
            /// 
            /// 173.234.226.10:8800
            /// 173.234.165.33:8800
            /// 
            Data.AirBnbAccounts.AddAccount("oliverrentals1@gmail.com", "lolpol98", new List<string>() { "173.234.226.10:8800", "173.234.165.33:8800" });

            //Data.AirBnbAccounts.AddAccount("", "lolpol98", new List<string>() { "", "" });
            //Data.AirBnbAccounts.AddAccount("", "", new List<string>() { "", "" });

            //*/

            /*

            //Unused proxy IP

            [
              "69.147.248.209:8800",
              "173.234.59.186:8800",
              "173.232.7.154:8800",
              "173.234.59.55:8800",
              "206.214.93.250:8800",
              "173.234.165.52:8800",
              "50.31.9.142:8800",
              "173.234.226.48:8800",
              "206.214.93.183:8800",
              "173.234.165.53:8800",
              "173.234.59.53:8800",
              "69.147.248.112:8800",
              "173.234.226.5:8800",
              "173.232.7.98:8800"
            ]

            // Reserved as second defulat is
                    173.234.165.33:8800

            */
        }
    }
}