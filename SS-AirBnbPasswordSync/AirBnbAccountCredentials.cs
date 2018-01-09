namespace SS_AirBnbPasswordSync
{
    using System.Collections.Generic;

    public class AirBnbAccountCredentials
    {
        public AirBnbAccountCredentials()
        {
            Proxies = new List<string>();
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Proxies { get; set; } 
    }
}